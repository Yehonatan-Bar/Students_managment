<!-- 
  Vue Single File Component (SFC) Structure:
  A .vue file has three main sections:
  1. <template> - HTML structure (what the user sees)
  2. <script> - JavaScript/TypeScript logic (how the component behaves)
  3. <style> - CSS styling (how it looks)
-->

<template>
  <div class="student-list-container">
    <h1>Student Management System</h1>
    
    <!-- v-if, v-else-if, v-else are Vue's conditional rendering directives -->
    <!-- Only one of these blocks will be shown based on the conditions -->
    
    <!-- Show loading spinner when data is being fetched -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading students...</p>
    </div>

    <!-- Show error message if something went wrong -->
    <div v-else-if="error" class="error-state">
      <!-- {{ }} is Vue's interpolation syntax - displays JavaScript values in HTML -->
      <p class="error-message">{{ error }}</p>
      <!-- @click is Vue's way of handling click events (shorthand for v-on:click) -->
      <button @click="fetchStudents" class="retry-button">Retry</button>
    </div>

    <!-- Show message when no students exist -->
    <div v-else-if="students.length === 0" class="empty-state">
      <p>No students found</p>
    </div>

    <!-- Show the student table when we have data -->
    <div v-else class="table-container">
      <!-- Fixed header that stays at top when scrolling -->
      <div class="table-header">
        <table class="students-table">
          <thead>
            <tr>
              <th style="width: 10%">ID</th>
              <th style="width: 30%">Full Name</th>
              <th style="width: 20%">Birth Date</th>
              <th style="width: 20%">Average Grade</th>
              <th style="width: 20%">Status</th>
            </tr>
          </thead>
        </table>
      </div>
      
      <!-- Virtual scrolling container - only renders visible rows for performance -->
      <!-- ref="parentRef" creates a reference to this DOM element that we can access in JavaScript -->
      <div ref="parentRef" class="virtual-scroll-container">
        <!-- This div has the total height of all rows to maintain proper scrollbar -->
        <div
          v-if="rowVirtualizer"
          :style="{
            height: `${rowVirtualizer.getTotalSize()}px`,
            position: 'relative',
          }"
        >
          <!-- v-for is Vue's way of looping through arrays -->
          <!-- Only renders the rows that are currently visible in the viewport -->
          <div
            v-for="virtualRow in rowVirtualizer.getVirtualItems()"
            :key="virtualRow.index"
            :style="{
              position: 'absolute',
              top: 0,
              left: 0,
              width: '100%',
              height: `${virtualRow.size}px`,
              transform: `translateY(${virtualRow.start}px)`,
            }"
          >
            <table class="students-table">
              <tbody>
                <tr :class="{'long-name': students[virtualRow.index].fullName.length > 30}">
                  <!-- Access student data using the virtual row's index -->
                  <td style="width: 10%">{{ students[virtualRow.index].studentId }}</td>
                  <td style="width: 30%" class="name-cell">{{ students[virtualRow.index].fullName }}</td>
                  <!-- Call formatDate function to format the date nicely -->
                  <td style="width: 20%">{{ formatDate(students[virtualRow.index].birthDate) }}</td>
                  <!-- toFixed(2) ensures we always show 2 decimal places -->
                  <td style="width: 20%">{{ students[virtualRow.index].averageGrade.toFixed(2) }}</td>
                  <td style="width: 20%">
                    <!-- :class is Vue's way to dynamically add CSS classes -->
                    <!-- The array syntax allows multiple classes, with conditional logic -->
                    <span :class="['status', students[virtualRow.index].isActive ? 'active' : 'inactive']">
                      {{ students[virtualRow.index].isActive ? 'Active' : 'Inactive' }}
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
        
        <!-- Fallback table when virtual scrolling is not available -->
        <div v-else>
          <table class="students-table">
            <tbody>
              <tr v-for="student in students" :key="student.studentId">
                <td style="width: 10%">{{ student.studentId }}</td>
                <td style="width: 30%">{{ student.fullName }}</td>
                <td style="width: 20%">{{ formatDate(student.birthDate) }}</td>
                <td style="width: 20%">{{ student.averageGrade.toFixed(2) }}</td>
                <td style="width: 20%">
                  <span :class="['status', student.isActive ? 'active' : 'inactive']">
                    {{ student.isActive ? 'Active' : 'Inactive' }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        
        <!-- Loading indicator for infinite scroll -->
        <div v-if="isLoadingMore" class="loading-more">
          <div class="spinner-small"></div>
          <p>Loading more students...</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Vue 3 Composition API with <script setup> syntax
// This is the modern way to write Vue components - more concise than Options API

// Import Vue's reactive primitives and lifecycle hooks
import { ref, onMounted, computed, nextTick } from 'vue';
// Virtual scrolling library for performance with large lists
import { useVirtualizer } from '@tanstack/vue-virtual';
// TypeScript type definition for our Student data
import type { Student } from '../types/student';
// Our API service for fetching student data
import { studentApi } from '../services/api';
// Logger utility for tracking operations
import { createLogger } from '../utils/logger';

// Create a logger instance for this component
const logger = createLogger('StudentManagement', 'StudentListVirtualComponent');

// ref() creates reactive variables that Vue will track for changes
// When these change, Vue automatically updates the UI
const students = ref<Student[]>([]);        // Array to hold student data
const loading = ref(true);                  // Loading state flag
const error = ref<string | null>(null);     // Error message (null when no error)
const parentRef = ref<HTMLElement>();       // Reference to the scroll container DOM element
const currentPage = ref(0);                 // Current page for infinite scroll
const hasMore = ref(true);                  // Whether there are more items to load
const isLoadingMore = ref(false);           // Loading state for additional items

// Function to estimate row height based on content
const estimateRowHeight = (index: number): number => {
  if (index >= students.value.length) return 50;
  
  const student = students.value[index];
  let baseHeight = 50;
  
  // Add extra height for long names
  if (student.fullName.length > 30) {
    baseHeight += 20;
  }
  
  // Add extra height for complex status text
  if (!student.isActive) {
    baseHeight += 5;
  }
  
  return baseHeight;
};

// computed() creates a reactive value that updates when its dependencies change
// This sets up the virtual scrolling logic
const rowVirtualizer = computed(() => {
  // Only create virtualizer if we have a parent element and students
  if (!parentRef.value || students.value.length === 0) {
    return null;
  }
  
  try {
    const virtualizer = useVirtualizer({
      count: students.value.length,           // Total number of items
      getScrollElement: () => parentRef.value, // The scrollable container
      estimateSize: estimateRowHeight,        // Dynamic height estimation
      overscan: 5,                            // Number of items to render outside visible area
    });
    
    // Ensure the virtualizer has the required methods
    if (virtualizer && typeof virtualizer.getTotalSize === 'function' && typeof virtualizer.getVirtualItems === 'function') {
      return virtualizer;
    }
    
    logger.warn('rowVirtualizer', { virtualizer }, 'Virtualizer missing required methods');
    return null;
  } catch (error) {
    logger.error('rowVirtualizer', { error }, 'Failed to initialize virtualizer');
    return null;
  }
});

// Function to format date strings into readable format
const formatDate = (dateString: string): string => {
  logger.debug('formatDate', { dateString });
  try {
    const date = new Date(dateString);
    // Convert to locale-specific date string
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  } catch (err) {
    // If date parsing fails, return original string
    logger.error('formatDate', { dateString, error: err }, 'Failed to format date');
    return dateString;
  }
};

// Async function to fetch student data from the API
const fetchStudents = async () => {
  logger.info('fetchStudents', {}, 'Starting to fetch students');
  loading.value = true;    // Show loading spinner
  error.value = null;      // Clear any previous errors
  
  try {
    // Call the API and wait for response
    students.value = await studentApi.getAllStudents();
    logger.info('fetchStudents', { count: students.value.length }, 'Successfully fetched students');
  } catch (err: any) {
    // Handle errors - try to get meaningful error message
    const errorMessage = err.response?.data?.message || err.message || 'Failed to fetch students';
    error.value = errorMessage;
    logger.error('fetchStudents', { error: err }, errorMessage);
  } finally {
    // Always hide loading spinner, whether success or failure
    loading.value = false;
  }
};

// Function to load more students with pagination
const loadMoreStudents = async () => {
  if (isLoadingMore.value || !hasMore.value) return;
  
  logger.info('loadMoreStudents', { currentPage: currentPage.value }, 'Loading more students');
  isLoadingMore.value = true;
  
  try {
    // Simulate paginated API call (would need backend support)
    const batchSize = 50;
    const startIndex = currentPage.value * batchSize;
    const allStudents = await studentApi.getAllStudents();
    
    // Get next batch
    const nextBatch = allStudents.slice(startIndex, startIndex + batchSize);
    
    if (nextBatch.length === 0) {
      hasMore.value = false;
      logger.info('loadMoreStudents', {}, 'No more students to load');
      return;
    }
    
    // Append new students to existing list
    students.value = [...students.value, ...nextBatch];
    currentPage.value += 1;
    
    // Check if we've reached the end
    if (students.value.length >= allStudents.length) {
      hasMore.value = false;
    }
    
    logger.info('loadMoreStudents', { 
      loadedCount: nextBatch.length, 
      totalCount: students.value.length 
    }, 'Successfully loaded more students');
    
  } catch (err: any) {
    logger.error('loadMoreStudents', { error: err }, 'Failed to load more students');
  } finally {
    isLoadingMore.value = false;
  }
};

// Function to handle scroll events for infinite scrolling
const handleScroll = () => {
  if (!parentRef.value) return;
  
  const { scrollTop, scrollHeight, clientHeight } = parentRef.value;
  const threshold = 200; // Load more when 200px from bottom
  
  if (scrollTop + clientHeight >= scrollHeight - threshold && hasMore.value && !isLoadingMore.value) {
    loadMoreStudents();
  }
};

// onMounted lifecycle hook - runs after component is added to the DOM
// This is where we typically fetch initial data
onMounted(() => {
  logger.info('onMounted', {}, 'Component mounted, fetching initial data');
  fetchStudents();
  
  // Add scroll event listener for infinite scrolling
  nextTick(() => {
    if (parentRef.value) {
      parentRef.value.addEventListener('scroll', handleScroll);
    }
  });
});
</script>

<!-- scoped attribute means these styles only apply to this component -->
<style scoped>
/* Container for the entire component */
.student-list-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

/* Page title styling */
h1 {
  color: #333;
  text-align: center;
  margin-bottom: 30px;
}

/* Centered content for loading, error, and empty states */
.loading-state,
.error-state,
.empty-state {
  text-align: center;
  padding: 40px;
}

/* Animated loading spinner */
.spinner {
  width: 40px;
  height: 40px;
  margin: 0 auto 20px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #3498db;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

/* Keyframe animation for spinner rotation */
@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Error message styling */
.error-message {
  color: #e74c3c;
  margin-bottom: 20px;
}

/* Retry button styling */
.retry-button {
  background-color: #3498db;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 16px;
}

/* Hover effect for retry button */
.retry-button:hover {
  background-color: #2980b9;
}

/* Main table container with shadow effect */
.table-container {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

/* Fixed header that stays visible when scrolling */
.table-header {
  position: sticky;
  top: 0;
  background: white;
  z-index: 10;
}

/* Virtual scroll container with fixed height */
.virtual-scroll-container {
  height: 600px;
  overflow: auto;
}

/* Table styling */
.students-table {
  width: 100%;
  border-collapse: collapse;
  table-layout: fixed;  /* Fixed layout for consistent column widths */
}

/* Table cell styling */
.students-table th,
.students-table td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #ddd;
}

/* Table header styling */
.students-table th {
  background-color: #f8f9fa;
  font-weight: 600;
  color: #333;
}

/* Row hover effect */
.students-table tr:hover {
  background-color: #f5f5f5;
}

/* Status badge styling */
.status {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 14px;
  font-weight: 500;
}

/* Active status - green */
.status.active {
  background-color: #d4edda;
  color: #155724;
}

/* Inactive status - red */
.status.inactive {
  background-color: #f8d7da;
  color: #721c24;
}

/* Variable height row styling */
.long-name {
  min-height: 70px;
}

.name-cell {
  word-wrap: break-word;
  overflow-wrap: break-word;
  max-width: 200px;
}

/* Loading more indicator */
.loading-more {
  text-align: center;
  padding: 20px;
  border-top: 1px solid #ddd;
}

.spinner-small {
  width: 20px;
  height: 20px;
  margin: 0 auto 10px;
  border: 2px solid #f3f3f3;
  border-top: 2px solid #3498db;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

/* Responsive design for mobile devices */
@media (max-width: 768px) {
  .student-list-container {
    padding: 10px;
  }
  
  .virtual-scroll-container {
    height: 400px;  /* Smaller height on mobile */
  }
  
  .students-table {
    font-size: 14px;
  }
  
  .students-table th,
  .students-table td {
    padding: 8px 10px;
  }
}
</style>