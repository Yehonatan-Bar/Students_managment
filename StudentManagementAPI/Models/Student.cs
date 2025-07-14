using System;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementAPI.Models
{
    /// <summary>
    /// Represents a student entity in the system.
    /// This model includes validation attributes to ensure data integrity.
    /// </summary>
    public class Student
    {
        /// <summary>
        /// Gets or sets the unique identifier for the student.
        /// This serves as the primary key in the database.
        /// </summary>
        [Key]
        public int StudentId { get; set; }

        /// <summary>
        /// Gets or sets the full name of the student.
        /// Must be between 2 and 100 characters long.
        /// </summary>
        /// <remarks>
        /// This field is required and will be validated on model binding.
        /// Empty strings are prevented by the default value assignment.
        /// </remarks>
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the birth date of the student.
        /// Used to calculate age and verify enrollment eligibility.
        /// </summary>
        /// <remarks>
        /// The DataType attribute ensures proper date formatting in API responses.
        /// </remarks>
        [Required(ErrorMessage = "Birth date is required")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Gets or sets the average grade of the student.
        /// Must be a value between 0 and 100 (percentage).
        /// </summary>
        /// <remarks>
        /// This represents the overall academic performance of the student.
        /// The Range attribute ensures the grade stays within valid bounds.
        /// </remarks>
        [Required(ErrorMessage = "Average grade is required")]
        [Range(0, 100, ErrorMessage = "Average grade must be between 0 and 100")]
        public double AverageGrade { get; set; }

        /// <summary>
        /// Gets or sets whether the student is currently active in the system.
        /// True indicates an enrolled student, false indicates inactive/graduated/withdrawn.
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
    }
}