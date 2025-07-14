// TypeScript interface for the Student data model
// This defines the shape of student objects throughout the application
// Interfaces in TypeScript provide type safety and IntelliSense support

export interface Student {
  studentId: number;      // Unique identifier for each student
  fullName: string;       // Student's complete name
  birthDate: string;      // Date of birth in ISO string format (e.g., "2000-01-15")
  averageGrade: number;   // Student's average grade (decimal number)
  isActive: boolean;      // Whether the student is currently active/enrolled
}