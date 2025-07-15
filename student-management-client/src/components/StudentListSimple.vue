<template>
  <div class="student-list-container">
    <div class="header-section">
      <h1 class="main-title">
        <span class="title-icon">🎓</span>
        Student Management System
      </h1>
      <p class="subtitle">Manage your students with ease and elegance</p>
      <p class="hebrew-subtitle">מערכות מידע - כל מה שצריך לדעת</p>
      <div class="social-links">
        <a href="https://www.linkedin.com/in/john-bar-42722921/" target="_blank" rel="noopener noreferrer" class="linkedin-link">
          <svg class="linkedin-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
            <path d="M19 0h-14c-2.761 0-5 2.239-5 5v14c0 2.761 2.239 5 5 5h14c2.762 0 5-2.239 5-5v-14c0-2.761-2.238-5-5-5zm-11 19h-3v-11h3v11zm-1.5-12.268c-.966 0-1.75-.79-1.75-1.764s.784-1.764 1.75-1.764 1.75.79 1.75 1.764-.783 1.764-1.75 1.764zm13.5 12.268h-3v-5.604c0-3.368-4-3.113-4 0v5.604h-3v-11h3v1.765c1.396-2.586 7-2.777 7 2.476v6.759z"/>
          </svg>
          Connect Yoni on LinkedIn
        </a>
      </div>
    </div>
    
    <!-- Loading state -->
    <div v-if="loading" class="loading-state">
      <div class="modern-spinner">
        <div class="spinner-ring"></div>
        <div class="spinner-ring"></div>
        <div class="spinner-ring"></div>
      </div>
      <p class="loading-text">Loading students...</p>
    </div>

    <!-- Error state -->
    <div v-else-if="error" class="error-state">
      <div class="error-icon">⚠️</div>
      <p class="error-message">{{ error }}</p>
      <button @click="fetchStudents" class="retry-button">
        <span class="button-icon">🔄</span>
        Retry
      </button>
    </div>

    <!-- Empty state -->
    <div v-else-if="students.length === 0" class="empty-state">
      <div class="empty-icon">📚</div>
      <h3>No students found</h3>
      <p class="empty-description">Start by adding your first student to the system</p>
    </div>

    <!-- Students table -->
    <div v-else class="table-container">
      <div class="table-header">
        <h2 class="table-title">Student Records</h2>
        <div class="student-count">{{ students.length }} students</div>
      </div>
      <div class="table-wrapper">
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
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  min-height: 100vh;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.header-section {
  text-align: center;
  margin-bottom: 40px;
  padding: 30px 0;
}

.main-title {
  color: white;
  font-size: 3rem;
  font-weight: 700;
  margin-bottom: 10px;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 15px;
}

.title-icon {
  font-size: 3.5rem;
  filter: drop-shadow(2px 2px 4px rgba(0, 0, 0, 0.3));
}

.subtitle {
  color: rgba(255, 255, 255, 0.9);
  font-size: 1.2rem;
  font-weight: 300;
  margin: 0;
  text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.2);
}

.hebrew-subtitle {
  color: rgba(255, 255, 255, 0.95);
  font-size: 1.4rem;
  font-weight: 400;
  margin: 10px 0 20px;
  text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.2);
  font-family: 'Arial Hebrew', Arial, sans-serif;
  direction: rtl;
}

.social-links {
  margin-top: 20px;
}

.linkedin-link {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  background: rgba(255, 255, 255, 0.2);
  color: white;
  padding: 12px 24px;
  border-radius: 30px;
  text-decoration: none;
  font-weight: 500;
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.3);
}

.linkedin-link:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.2);
}

.linkedin-icon {
  width: 24px;
  height: 24px;
}

.loading-state,
.error-state,
.empty-state {
  text-align: center;
  padding: 60px 40px;
  background: white;
  border-radius: 20px;
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.1);
  margin: 20px;
}

.modern-spinner {
  display: inline-block;
  position: relative;
  width: 80px;
  height: 80px;
  margin-bottom: 30px;
}

.spinner-ring {
  box-sizing: border-box;
  display: block;
  position: absolute;
  width: 64px;
  height: 64px;
  margin: 8px;
  border: 8px solid transparent;
  border-radius: 50%;
  animation: ring-spin 1.2s cubic-bezier(0.5, 0, 0.5, 1) infinite;
}

.spinner-ring:nth-child(1) {
  border-top-color: #667eea;
  animation-delay: -0.45s;
}

.spinner-ring:nth-child(2) {
  border-top-color: #764ba2;
  animation-delay: -0.3s;
}

.spinner-ring:nth-child(3) {
  border-top-color: #f093fb;
  animation-delay: -0.15s;
}

@keyframes ring-spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

.loading-text {
  color: #555;
  font-size: 1.1rem;
  font-weight: 500;
}

.error-state {
  border-left: 5px solid #e74c3c;
}

.error-icon {
  font-size: 3rem;
  margin-bottom: 20px;
}

.error-message {
  color: #e74c3c;
  margin-bottom: 30px;
  font-size: 1.1rem;
  font-weight: 500;
}

.retry-button {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  padding: 12px 30px;
  border-radius: 25px;
  cursor: pointer;
  font-size: 16px;
  font-weight: 600;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
}

.retry-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.6);
}

.button-icon {
  font-size: 1.1rem;
}

.empty-state {
  border-left: 5px solid #3498db;
}

.empty-icon {
  font-size: 4rem;
  margin-bottom: 20px;
}

.empty-state h3 {
  color: #333;
  font-size: 1.5rem;
  margin-bottom: 10px;
}

.empty-description {
  color: #666;
  font-size: 1rem;
}

.table-container {
  background: white;
  border-radius: 20px;
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  margin: 20px;
}

.table-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 25px 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.table-title {
  color: white;
  font-size: 1.5rem;
  font-weight: 600;
  margin: 0;
  text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.2);
}

.student-count {
  color: rgba(255, 255, 255, 0.9);
  background: rgba(255, 255, 255, 0.2);
  padding: 8px 16px;
  border-radius: 20px;
  font-weight: 500;
  backdrop-filter: blur(10px);
}

.table-wrapper {
  overflow-x: auto;
}

.students-table {
  width: 100%;
  border-collapse: collapse;
}

.students-table th,
.students-table td {
  padding: 18px 20px;
  text-align: left;
  border-bottom: 1px solid #f0f0f0;
  color: #333;
}

.students-table th {
  background: linear-gradient(135deg, #f8f9ff 0%, #f0f2ff 100%);
  font-weight: 600;
  color: #5a6c7d;
  text-transform: uppercase;
  font-size: 0.85rem;
  letter-spacing: 0.5px;
}

.students-table tbody tr {
  transition: all 0.3s ease;
}

.students-table tbody tr:hover {
  background: linear-gradient(135deg, #f8f9ff 0%, #f0f2ff 100%);
  transform: scale(1.02);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
}

.students-table tbody tr:nth-child(even) {
  background-color: #fafbff;
}

.status {
  padding: 6px 16px;
  border-radius: 25px;
  font-size: 13px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  display: inline-block;
}

.status.active {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
  box-shadow: 0 2px 10px rgba(79, 172, 254, 0.4);
}

.status.inactive {
  background: linear-gradient(135deg, #fa709a 0%, #fee140 100%);
  color: white;
  box-shadow: 0 2px 10px rgba(250, 112, 154, 0.4);
}

@media (max-width: 768px) {
  .main-title {
    font-size: 2rem;
    flex-direction: column;
    gap: 10px;
  }
  
  .title-icon {
    font-size: 2.5rem;
  }
  
  .table-header {
    flex-direction: column;
    gap: 15px;
    text-align: center;
  }
  
  .students-table th,
  .students-table td {
    padding: 12px 8px;
    font-size: 0.9rem;
  }
}
</style>