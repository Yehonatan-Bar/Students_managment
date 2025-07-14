using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Data
{
    /// <summary>
    /// Database context for managing student entities using SQLite.
    /// This context provides access to the Students table and configures the database schema.
    /// </summary>
    public class StudentDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the StudentDbContext class.
        /// </summary>
        /// <param name="options">The options to configure the context.</param>
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the DbSet for managing Student entities.
        /// </summary>
        public DbSet<Student> Students { get; set; } = null!;

        /// <summary>
        /// Configures the model and database schema.
        /// This method is called when the model is being created.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Student entity
            modelBuilder.Entity<Student>(entity =>
            {
                // Configure primary key
                entity.HasKey(e => e.StudentId);

                // Configure StudentId as identity column
                entity.Property(e => e.StudentId)
                    .ValueGeneratedOnAdd();

                // Configure FullName column
                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

                // Configure BirthDate column
                entity.Property(e => e.BirthDate)
                    .IsRequired()
                    .HasColumnType("DATE");

                // Configure AverageGrade column
                entity.Property(e => e.AverageGrade)
                    .IsRequired()
                    .HasColumnType("REAL");

                // Configure IsActive column
                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                // Create index on StudentId for better performance
                entity.HasIndex(e => e.StudentId)
                    .HasDatabaseName("IX_Students_StudentId");

                // Create index on IsActive for filtering queries
                entity.HasIndex(e => e.IsActive)
                    .HasDatabaseName("IX_Students_IsActive");

                // Create composite index for active students ordered by name
                entity.HasIndex(e => new { e.IsActive, e.FullName })
                    .HasDatabaseName("IX_Students_IsActive_FullName");

                // Create composite index for active students ordered by grade
                entity.HasIndex(e => new { e.IsActive, e.AverageGrade })
                    .HasDatabaseName("IX_Students_IsActive_AverageGrade");

                // Create composite index for students by birth date range
                entity.HasIndex(e => new { e.IsActive, e.BirthDate })
                    .HasDatabaseName("IX_Students_IsActive_BirthDate");

                // Configure table name
                entity.ToTable("Students");
            });
        }
    }
}