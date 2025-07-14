<template>
  <div class="student-list-container">
    <h1>Student Management System</h1>
    
    <!-- Loading state -->
    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading students...</p>
    </div>

    <!-- Error state -->
    <div v-else-if="error" class="error-state">
      <p class="error-message">{{ error }}</p>
      <button @click="fetchStudents" class="retry-button">Retry</button>
    </div>

    <!-- Empty state -->
    <div v-else-if="students.length === 0" class="empty-state">
      <p>No students found</p>
    </div>

    <!-- Students table -->
    <div v-else class="table-container">
      <table class="students-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Full Name</th>
            <th>Birth Date</th>
            <th>Average Grade</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="student in students" :key="student.studentId">
            <td>{{ student.studentId }}</td>
            <td>{{ student.fullName }}</td>
            <td>{{ formatDate(student.birthDate) }}</td>
            <td>{{ student.averageGrade.toFixed(2) }}</td>
            <td>
              <span :class="['status', student.isActive ? 'active' : 'inactive']">
                {{ student.isActive ? 'Active' : 'Inactive' }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import type { Student } from '../types/student';
import { studentApi } from '../services/api';
import { createLogger } from '../utils/logger';

const logger = createLogger('StudentManagement', 'StudentListSimpleComponent');

const students = ref<Student[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);

const formatDate = (dateString: string): string => {
  try {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  } catch (err) {
    return dateString;
  }
};

const fetchStudents = async () => {
  logger.info('fetchStudents', {}, 'Starting to fetch students');
  loading.value = true;
  error.value = null;
  
  try {
    students.value = await studentApi.getAllStudents();
    logger.info('fetchStudents', { count: students.value.length }, 'Successfully fetched students');
  } catch (err: any) {
    const errorMessage = err.response?.data?.message || err.message || 'Failed to fetch students';
    error.value = errorMessage;
    logger.error('fetchStudents', { error: err }, errorMessage);
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  logger.info('onMounted', {}, 'Component mounted, fetching initial data');
  fetchStudents();
});
</script>

<style scoped>
.student-list-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

h1 {
  color: #333;
  text-align: center;
  margin-bottom: 30px;
}

.loading-state,
.error-state,
.empty-state {
  text-align: center;
  padding: 40px;
}

.spinner {
  width: 40px;
  height: 40px;
  margin: 0 auto 20px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #3498db;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error-message {
  color: #e74c3c;
  margin-bottom: 20px;
}

.retry-button {
  background-color: #3498db;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 16px;
}

.retry-button:hover {
  background-color: #2980b9;
}

.table-container {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  overflow-x: auto;
}

.students-table {
  width: 100%;
  border-collapse: collapse;
}

.students-table th,
.students-table td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #ddd;
  color: #333; /* Add text color */
}

.students-table th {
  background-color: #f8f9fa;
  font-weight: 600;
  color: #333;
}

.students-table tbody tr:hover {
  background-color: #f5f5f5;
}

.status {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 14px;
  font-weight: 500;
}

.status.active {
  background-color: #d4edda;
  color: #155724;
}

.status.inactive {
  background-color: #f8d7da;
  color: #721c24;
}
</style>