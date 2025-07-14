using System;

namespace StudentManagementAPI.Models
{
    /// <summary>
    /// Data Transfer Object (DTO) for Student entity.
    /// Used for API responses to control what data is exposed to clients.
    /// </summary>
    /// <remarks>
    /// This DTO pattern provides several benefits:
    /// - Decouples internal domain models from API contracts
    /// - Prevents over-posting attacks by limiting exposed properties
    /// - Allows API versioning without affecting domain models
    /// - Provides a clean, flat structure for JSON serialization
    /// </remarks>
    public class StudentDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the student.
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        /// Gets or sets the full name of the student.
        /// Initialized to empty string to prevent null reference issues.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the birth date of the student.
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Gets or sets the average grade of the student (0-100).
        /// </summary>
        public double AverageGrade { get; set; }

        /// <summary>
        /// Gets or sets whether the student is currently active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}