using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Services
{
    /// <summary>
    /// Defines the contract for student-related business operations.
    /// </summary>
    /// <remarks>
    /// This interface follows the repository pattern and provides an abstraction layer
    /// between the controller and data access logic. It enables:
    /// - Dependency injection for loose coupling
    /// - Easy unit testing through mocking
    /// - Flexibility to switch between different implementations (in-memory, database, etc.)
    /// All methods are asynchronous to support scalable, non-blocking operations.
    /// </remarks>
    public interface IStudentService
    {
        /// <summary>
        /// Retrieves all students from the data store.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a collection of all students as DTOs.
        /// Returns an empty collection if no students exist.
        /// </returns>
        Task<IEnumerable<StudentDto>> GetAllStudentsAsync();

        /// <summary>
        /// Retrieves a specific student by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the student to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the student DTO if found; otherwise, null.
        /// </returns>
        Task<StudentDto?> GetStudentByIdAsync(int id);

        /// <summary>
        /// Creates a new student in the data store.
        /// </summary>
        /// <param name="student">The student entity to create. The StudentId will be auto-generated.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the created student as a DTO with the assigned ID.
        /// </returns>
        /// <remarks>
        /// The implementation should handle ID generation and ensure the student
        /// passes all validation rules before persisting.
        /// </remarks>
        Task<StudentDto> CreateStudentAsync(Student student);

        /// <summary>
        /// Updates an existing student's information.
        /// </summary>
        /// <param name="id">The unique identifier of the student to update.</param>
        /// <param name="student">The updated student information.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the updated student DTO if successful; otherwise, null if not found.
        /// </returns>
        /// <remarks>
        /// The implementation should ensure the student exists before attempting update
        /// and maintain the original StudentId.
        /// </remarks>
        Task<StudentDto?> UpdateStudentAsync(int id, Student student);

        /// <summary>
        /// Deletes a student from the data store.
        /// </summary>
        /// <param name="id">The unique identifier of the student to delete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains true if the deletion was successful; false if the student was not found.
        /// </returns>
        /// <remarks>
        /// This performs a hard delete. In production scenarios, consider implementing
        /// soft delete by setting IsActive to false instead.
        /// </remarks>
        Task<bool> DeleteStudentAsync(int id);
    }
}