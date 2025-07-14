using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentManagementAPI.Data;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Services
{
    /// <summary>
    /// Entity Framework implementation of the IStudentService interface.
    /// Provides business logic for student management operations using SQLite database.
    /// </summary>
    /// <remarks>
    /// This implementation uses Entity Framework Core with SQLite for data persistence.
    /// All operations are async and properly handle database transactions.
    /// </remarks>
    public class StudentService : IStudentService
    {
        /// <summary>
        /// Database context for Entity Framework operations.
        /// </summary>
        private readonly StudentDbContext _context;
        
        /// <summary>
        /// Logger instance for tracking service operations.
        /// </summary>
        private readonly ILogger<StudentService> _logger;

        /// <summary>
        /// Cache service for storing frequently accessed data.
        /// </summary>
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Initializes a new instance of the StudentService class.
        /// </summary>
        /// <param name="context">The database context for Entity Framework operations.</param>
        /// <param name="logger">The logger instance for operation tracking.</param>
        /// <param name="cacheService">The cache service for storing frequently accessed data.</param>
        public StudentService(StudentDbContext context, ILogger<StudentService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }


        /// <summary>
        /// Retrieves all students from the database.
        /// </summary>
        /// <returns>A collection of all students as DTOs.</returns>
        /// <remarks>
        /// This method performs a projection from Student entities to StudentDto objects
        /// to ensure proper data encapsulation. Uses Entity Framework's async query capabilities.
        /// </remarks>
        public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
        {
            const string cacheKey = "students:all";
            
            _logger.LogInformation("[StudentService][GetAllStudentsAsync] Checking cache for all students");
            
            var cachedStudents = await _cacheService.GetAsync<IEnumerable<StudentDto>>(cacheKey);
            if (cachedStudents != null)
            {
                _logger.LogInformation($"[StudentService][GetAllStudentsAsync] Retrieved {cachedStudents.Count()} students from cache");
                return cachedStudents;
            }
            
            _logger.LogInformation("[StudentService][GetAllStudentsAsync] Retrieving all students from database");
            
            var students = await _context.Students
                .Select(s => new StudentDto
                {
                    StudentId = s.StudentId,
                    FullName = s.FullName,
                    BirthDate = s.BirthDate,
                    AverageGrade = s.AverageGrade,
                    IsActive = s.IsActive
                })
                .ToListAsync();

            _logger.LogInformation($"[StudentService][GetAllStudentsAsync] Retrieved {students.Count} students from database");
            
            await _cacheService.SetAsync(cacheKey, students, TimeSpan.FromMinutes(5));
            _logger.LogInformation("[StudentService][GetAllStudentsAsync] Cached students for 5 minutes");
            
            return students;
        }

        /// <summary>
        /// Retrieves a specific student by their unique identifier.
        /// </summary>
        /// <param name="id">The student's unique identifier.</param>
        /// <returns>The student DTO if found; otherwise, null.</returns>
        /// <remarks>
        /// Uses Entity Framework's async query capabilities for efficient database access.
        /// Returns null following the nullable reference pattern for not-found scenarios.
        /// </remarks>
        public async Task<StudentDto?> GetStudentByIdAsync(int id)
        {
            var cacheKey = $"student:{id}";
            
            _logger.LogInformation($"[StudentService][GetStudentByIdAsync] Checking cache for student ID: {id}");
            
            var cachedStudent = await _cacheService.GetAsync<StudentDto>(cacheKey);
            if (cachedStudent != null)
            {
                _logger.LogInformation($"[StudentService][GetStudentByIdAsync] Retrieved student from cache: {cachedStudent.FullName}");
                return cachedStudent;
            }
            
            _logger.LogInformation($"[StudentService][GetStudentByIdAsync] Retrieving student with ID: {id} from database");
            
            var student = await _context.Students
                .Where(s => s.StudentId == id)
                .Select(s => new StudentDto
                {
                    StudentId = s.StudentId,
                    FullName = s.FullName,
                    BirthDate = s.BirthDate,
                    AverageGrade = s.AverageGrade,
                    IsActive = s.IsActive
                })
                .FirstOrDefaultAsync();

            if (student == null)
            {
                _logger.LogWarning($"[StudentService][GetStudentByIdAsync] Student with ID {id} not found in database");
                return null;
            }

            _logger.LogInformation($"[StudentService][GetStudentByIdAsync] Found student: {student.FullName}");
            
            await _cacheService.SetAsync(cacheKey, student, TimeSpan.FromMinutes(10));
            _logger.LogInformation($"[StudentService][GetStudentByIdAsync] Cached student {student.FullName} for 10 minutes");
            
            return student;
        }

        /// <summary>
        /// Creates a new student in the database.
        /// </summary>
        /// <param name="student">The student entity to create.</param>
        /// <returns>The created student as a DTO with assigned ID.</returns>
        /// <remarks>
        /// The StudentId is auto-generated by the database. The method assumes the student
        /// has already passed validation at the controller level.
        /// </remarks>
        public async Task<StudentDto> CreateStudentAsync(Student student)
        {
            _logger.LogInformation($"[StudentService][CreateStudentAsync] Creating new student: {student.FullName}");
            
            // Reset the ID to allow Entity Framework to auto-generate it
            student.StudentId = 0;
            
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"[StudentService][CreateStudentAsync] Created student with ID: {student.StudentId}");
            
            var studentDto = new StudentDto
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                BirthDate = student.BirthDate,
                AverageGrade = student.AverageGrade,
                IsActive = student.IsActive
            };
            
            await InvalidateStudentCacheAsync(student.StudentId);
            _logger.LogInformation($"[StudentService][CreateStudentAsync] Invalidated cache after creating student");
            
            return studentDto;
        }

        /// <summary>
        /// Updates an existing student's information in the database.
        /// </summary>
        /// <param name="id">The ID of the student to update.</param>
        /// <param name="student">The updated student information.</param>
        /// <returns>The updated student DTO if successful; null if not found.</returns>
        /// <remarks>
        /// This method performs a full update of all student properties except the ID.
        /// Partial updates are not supported in this implementation.
        /// The original StudentId is preserved to maintain referential integrity.
        /// </remarks>
        public async Task<StudentDto?> UpdateStudentAsync(int id, Student student)
        {
            _logger.LogInformation($"[StudentService][UpdateStudentAsync] Updating student with ID: {id}");
            
            var existingStudent = await _context.Students.FindAsync(id);
            if (existingStudent == null)
            {
                _logger.LogWarning($"[StudentService][UpdateStudentAsync] Student with ID {id} not found in database");
                return null;
            }

            existingStudent.FullName = student.FullName;
            existingStudent.BirthDate = student.BirthDate;
            existingStudent.AverageGrade = student.AverageGrade;
            existingStudent.IsActive = student.IsActive;
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"[StudentService][UpdateStudentAsync] Updated student: {existingStudent.FullName}");
            
            var studentDto = new StudentDto
            {
                StudentId = existingStudent.StudentId,
                FullName = existingStudent.FullName,
                BirthDate = existingStudent.BirthDate,
                AverageGrade = existingStudent.AverageGrade,
                IsActive = existingStudent.IsActive
            };
            
            await InvalidateStudentCacheAsync(id);
            _logger.LogInformation($"[StudentService][UpdateStudentAsync] Invalidated cache after updating student");
            
            return studentDto;
        }

        /// <summary>
        /// Permanently removes a student from the database.
        /// </summary>
        /// <param name="id">The ID of the student to delete.</param>
        /// <returns>True if deletion was successful; false if student not found.</returns>
        /// <remarks>
        /// This performs a hard delete from the database.
        /// In production systems, consider implementing soft delete by setting
        /// IsActive to false to maintain audit trails and referential integrity.
        /// </remarks>
        public async Task<bool> DeleteStudentAsync(int id)
        {
            _logger.LogInformation($"[StudentService][DeleteStudentAsync] Deleting student with ID: {id}");
            
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                _logger.LogWarning($"[StudentService][DeleteStudentAsync] Student with ID {id} not found in database");
                return false;
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"[StudentService][DeleteStudentAsync] Deleted student: {student.FullName}");
            
            await InvalidateStudentCacheAsync(id);
            _logger.LogInformation($"[StudentService][DeleteStudentAsync] Invalidated cache after deleting student");
            
            return true;
        }

        /// <summary>
        /// Invalidates cached data for a specific student and all related cache entries.
        /// </summary>
        /// <param name="studentId">The ID of the student whose cache should be invalidated.</param>
        private async Task InvalidateStudentCacheAsync(int studentId)
        {
            await _cacheService.RemoveAsync($"student:{studentId}");
            await _cacheService.RemoveAsync("students:all");
            await _cacheService.RemoveByPatternAsync("students:*");
        }
    }
}