using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Data
{
    /// <summary>
    /// Provides methods for seeding initial data into the database.
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// Seeds the database with sample student data for testing and demonstration.
        /// </summary>
        /// <param name="context">The database context to seed.</param>
        /// <param name="logger">Logger for tracking seeding operations.</param>
        public static async Task SeedAsync(StudentDbContext context, ILogger logger)
        {
            // Ensure the database is created
            await context.Database.EnsureCreatedAsync();

            // Check if we already have data
            if (await context.Students.AnyAsync())
            {
                logger.LogInformation("[SeedData] Database already contains student data, skipping seeding");
                return;
            }

            logger.LogInformation("[SeedData] Seeding database with sample student data");

            var students = new[]
            {
                new Student 
                { 
                    FullName = "John Doe", 
                    BirthDate = new DateTime(2000, 1, 15), 
                    AverageGrade = 85.5, 
                    IsActive = true 
                },
                new Student 
                { 
                    FullName = "Jane Smith", 
                    BirthDate = new DateTime(1999, 5, 20), 
                    AverageGrade = 92.3, 
                    IsActive = true 
                },
                new Student 
                { 
                    FullName = "Bob Johnson", 
                    BirthDate = new DateTime(2001, 8, 10), 
                    AverageGrade = 78.9, 
                    IsActive = false 
                },
                new Student 
                { 
                    FullName = "Alice Williams", 
                    BirthDate = new DateTime(2000, 12, 5), 
                    AverageGrade = 95.7, 
                    IsActive = true 
                },
                new Student 
                { 
                    FullName = "Charlie Brown", 
                    BirthDate = new DateTime(1998, 3, 25), 
                    AverageGrade = 81.2, 
                    IsActive = true 
                },
                new Student 
                { 
                    FullName = "Emma Davis", 
                    BirthDate = new DateTime(2001, 7, 8), 
                    AverageGrade = 89.4, 
                    IsActive = true 
                },
                new Student 
                { 
                    FullName = "Michael Wilson", 
                    BirthDate = new DateTime(1999, 11, 12), 
                    AverageGrade = 76.8, 
                    IsActive = true 
                },
                new Student 
                { 
                    FullName = "Sophia Taylor", 
                    BirthDate = new DateTime(2000, 4, 18), 
                    AverageGrade = 93.1, 
                    IsActive = false 
                },
                new Student 
                { 
                    FullName = "James Anderson", 
                    BirthDate = new DateTime(1998, 9, 30), 
                    AverageGrade = 87.6, 
                    IsActive = true 
                },
                new Student 
                { 
                    FullName = "Isabella Martinez", 
                    BirthDate = new DateTime(2001, 2, 14), 
                    AverageGrade = 91.5, 
                    IsActive = true 
                }
            };

            await context.Students.AddRangeAsync(students);
            await context.SaveChangesAsync();

            logger.LogInformation($"[SeedData] Successfully seeded {students.Length} students into the database");
        }
    }
}