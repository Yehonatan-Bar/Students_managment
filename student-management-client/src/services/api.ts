// This file handles all API communication between the Vue frontend and the backend server
// It uses Axios, a popular HTTP client library for making API requests

import axios from 'axios';
import type { Student } from '../types/student';
import { createLogger } from '../utils/logger';

// Create a logger instance for this module
// This will tag all logs with 'StudentManagement' as the feature and 'ApiService' as the module
const logger = createLogger('StudentManagement', 'ApiService');

// Get the API base URL from environment variables (defined in .env file)
// import.meta.env is Vite's way of accessing environment variables
// VITE_ prefix is required for Vite to expose the variable to the client
// Falls back to http://localhost:5000/api if not defined
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

// Create a configured instance of axios with default settings
// This instance will be used for all API calls instead of the global axios
const apiClient = axios.create({
  baseURL: API_BASE_URL,      // All requests will be prefixed with this URL
  timeout: 10000,             // Request will timeout after 10 seconds
  headers: {
    'Content-Type': 'application/json',  // Tell server we're sending JSON data
  },
});

// Interceptors are middleware that run before requests are sent or after responses are received
// Request interceptor - runs before every API request
apiClient.interceptors.request.use(
  (config) => {
    // Log details about the outgoing request
    logger.info('apiClient.interceptors.request', {
      url: config.url,
      method: config.method,
      data: config.data,
    });
    return config;  // Must return config to continue with the request
  },
  (error) => {
    // Log any errors that occur while preparing the request
    logger.error('apiClient.interceptors.request', {
      error: error.message,
    });
    return Promise.reject(error);  // Pass the error to the catch block
  }
);

// Response interceptor - runs after every API response
apiClient.interceptors.response.use(
  (response) => {
    // Log successful responses
    logger.info('apiClient.interceptors.response', {
      url: response.config.url,
      status: response.status,
      // If response is an array, log its length; otherwise log 1
      dataLength: Array.isArray(response.data) ? response.data.length : 1,
    });
    return response;  // Must return response to pass it to the calling code
  },
  (error) => {
    // Log error responses (4xx, 5xx status codes, network errors, etc.)
    logger.error('apiClient.interceptors.response', {
      url: error.config?.url,         // Optional chaining (?.) prevents errors if config is undefined
      status: error.response?.status,  // HTTP status code if available
      message: error.message,
    });
    return Promise.reject(error);  // Pass the error to the catch block
  }
);

// Export an object containing all API methods
// This pattern creates a namespace for related API calls
export const studentApi = {
  // Fetch all students from the backend
  // Returns a Promise that resolves to an array of Student objects
  async getAllStudents(): Promise<Student[]> {
    try {
      logger.info('getAllStudents', {}, 'Fetching all students');
      // Make GET request to /students endpoint
      // The <Student[]> is a TypeScript generic that tells axios what type of data to expect
      const response = await apiClient.get<Student[]>('/students');
      return response.data;  // axios stores the actual response data in the .data property
    } catch (error) {
      logger.error('getAllStudents', { error }, 'Failed to fetch students');
      throw error;  // Re-throw the error so the component can handle it
    }
  },

  // Fetch a single student by their ID
  // Returns a Promise that resolves to a single Student object
  async getStudentById(id: number): Promise<Student> {
    try {
      logger.info('getStudentById', { id }, 'Fetching student by ID');
      // Template literal (backticks) allows embedding the id variable in the URL
      const response = await apiClient.get<Student>(`/students/${id}`);
      return response.data;
    } catch (error) {
      logger.error('getStudentById', { id, error }, 'Failed to fetch student');
      throw error;  // Re-throw the error so the component can handle it
    }
  },
};