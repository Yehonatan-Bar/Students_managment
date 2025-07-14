using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentManagementAPI.Models;
using StudentManagementAPI.Services;

namespace StudentManagementAPI.Controllers
{
    /// <summary>
    /// RESTful API controller for managing student resources.
    /// </summary>
    /// <remarks>
    /// This controller follows REST conventions:
    /// - GET for retrieval operations
    /// - POST for creation
    /// - PUT for updates
    /// - DELETE for removal
    /// All actions return appropriate HTTP status codes and include comprehensive error handling.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        /// <summary>
        /// Student service instance for business logic operations.
        /// </summary>
        private readonly IStudentService _studentService;
        
        /// <summary>
        /// Logger instance for tracking controller operations.
        /// </summary>
        private readonly ILogger<StudentsController> _logger;

        /// <summary>
        /// Initializes a new instance of the StudentsController.
        /// </summary>
        /// <param name="studentService">The student service for business operations.</param>
        /// <param name="logger">The logger for operation tracking.</param>
        /// <remarks>
        /// Dependencies are injected via ASP.NET Core's built-in DI container.
        /// </remarks>
        public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all students from the system.
        /// </summary>
        /// <returns>A list of all students.</returns>
        /// <response code="200">Returns the list of students</response>
        /// <response code="500">If an internal server error occurs</response>
        /// <remarks>
        /// GET api/students
        /// Returns an empty array if no students exist.
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            try
            {
                _logger.LogInformation("[StudentsController][GetStudents] GET request for all students");
                var students = await _studentService.GetAllStudentsAsync();
                _logger.LogInformation($"[StudentsController][GetStudents] Returning {((List<StudentDto>)students).Count} students");
                return Ok(students);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StudentsController][GetStudents] Error retrieving students");
                return StatusCode(500, new { error = "An error occurred while retrieving students" });
            }
        }

        /// <summary>
        /// Retrieves a specific student by ID.
        /// </summary>
        /// <param name="id">The student's unique identifier.</param>
        /// <returns>The requested student.</returns>
        /// <response code="200">Returns the requested student</response>
        /// <response code="404">If the student is not found</response>
        /// <response code="500">If an internal server error occurs</response>
        /// <remarks>
        /// GET api/students/5
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            try
            {
                _logger.LogInformation($"[StudentsController][GetStudent] GET request for student ID: {id}");
                var student = await _studentService.GetStudentByIdAsync(id);
                
                if (student == null)
                {
                    _logger.LogWarning($"[StudentsController][GetStudent] Student with ID {id} not found");
                    return NotFound(new { error = $"Student with ID {id} not found" });
                }

                _logger.LogInformation($"[StudentsController][GetStudent] Returning student: {student.FullName}");
                return Ok(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[StudentsController][GetStudent] Error retrieving student with ID {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving the student" });
            }
        }

        /// <summary>
        /// Creates a new student.
        /// </summary>
        /// <param name="student">The student to create.</param>
        /// <returns>The created student with assigned ID.</returns>
        /// <response code="201">Returns the newly created student</response>
        /// <response code="400">If the student data is invalid</response>
        /// <response code="500">If an internal server error occurs</response>
        /// <remarks>
        /// POST api/students
        /// {
        ///   "fullName": "John Doe",
        ///   "birthDate": "2000-01-01",
        ///   "averageGrade": 85.5,
        ///   "isActive": true
        /// }
        /// The StudentId in the request body is ignored; a new ID is auto-generated.
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<StudentDto>> CreateStudent([FromBody] Student student)
        {
            try
            {
                _logger.LogInformation($"[StudentsController][CreateStudent] POST request to create student: {student.FullName}");
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("[StudentsController][CreateStudent] Invalid model state");
                    return BadRequest(ModelState);
                }

                var createdStudent = await _studentService.CreateStudentAsync(student);
                _logger.LogInformation($"[StudentsController][CreateStudent] Created student with ID: {createdStudent.StudentId}");
                
                return CreatedAtAction(nameof(GetStudent), new { id = createdStudent.StudentId }, createdStudent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StudentsController][CreateStudent] Error creating student");
                return StatusCode(500, new { error = "An error occurred while creating the student" });
            }
        }

        /// <summary>
        /// Updates an existing student.
        /// </summary>
        /// <param name="id">The ID of the student to update.</param>
        /// <param name="student">The updated student data.</param>
        /// <returns>The updated student.</returns>
        /// <response code="200">Returns the updated student</response>
        /// <response code="400">If the student data is invalid</response>
        /// <response code="404">If the student is not found</response>
        /// <response code="500">If an internal server error occurs</response>
        /// <remarks>
        /// PUT api/students/5
        /// {
        ///   "fullName": "John Doe Updated",
        ///   "birthDate": "2000-01-01",
        ///   "averageGrade": 90.0,
        ///   "isActive": true
        /// }
        /// This performs a full update; all fields must be provided.
        /// </remarks>
        [HttpPut("{id}")]
        public async Task<ActionResult<StudentDto>> UpdateStudent(int id, [FromBody] Student student)
        {
            try
            {
                _logger.LogInformation($"[StudentsController][UpdateStudent] PUT request to update student ID: {id}");
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("[StudentsController][UpdateStudent] Invalid model state");
                    return BadRequest(ModelState);
                }

                var updatedStudent = await _studentService.UpdateStudentAsync(id, student);
                
                if (updatedStudent == null)
                {
                    _logger.LogWarning($"[StudentsController][UpdateStudent] Student with ID {id} not found");
                    return NotFound(new { error = $"Student with ID {id} not found" });
                }

                _logger.LogInformation($"[StudentsController][UpdateStudent] Updated student: {updatedStudent.FullName}");
                return Ok(updatedStudent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[StudentsController][UpdateStudent] Error updating student with ID {id}");
                return StatusCode(500, new { error = "An error occurred while updating the student" });
            }
        }

        /// <summary>
        /// Deletes a student from the system.
        /// </summary>
        /// <param name="id">The ID of the student to delete.</param>
        /// <returns>No content on successful deletion.</returns>
        /// <response code="204">If the student was successfully deleted</response>
        /// <response code="404">If the student is not found</response>
        /// <response code="500">If an internal server error occurs</response>
        /// <remarks>
        /// DELETE api/students/5
        /// This performs a hard delete. Consider implementing soft delete
        /// for production systems to maintain data integrity.
        /// </remarks>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                _logger.LogInformation($"[StudentsController][DeleteStudent] DELETE request for student ID: {id}");
                var result = await _studentService.DeleteStudentAsync(id);
                
                if (!result)
                {
                    _logger.LogWarning($"[StudentsController][DeleteStudent] Student with ID {id} not found");
                    return NotFound(new { error = $"Student with ID {id} not found" });
                }

                _logger.LogInformation($"[StudentsController][DeleteStudent] Deleted student with ID: {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[StudentsController][DeleteStudent] Error deleting student with ID {id}");
                return StatusCode(500, new { error = "An error occurred while deleting the student" });
            }
        }
    }
}