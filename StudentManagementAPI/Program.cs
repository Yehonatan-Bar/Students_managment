using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.Services;
using static StudentManagementAPI.Data.SeedData;

// Create the web application builder with default configuration
var builder = WebApplication.CreateBuilder(args);

// ========== Service Configuration ==========
// Add MVC controllers for handling HTTP requests
// This enables the API to process incoming requests and return responses
builder.Services.AddControllers();

// Add API explorer services required for OpenAPI/Swagger documentation
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI for API documentation
// This provides an interactive UI for testing API endpoints
builder.Services.AddSwaggerGen();

// ========== Database Configuration ==========
// Configure Entity Framework with SQLite database
builder.Services.AddDbContext<StudentDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========== Cache Configuration ==========
// Configure Memory Cache for storing frequently accessed data
builder.Services.AddMemoryCache();

// Configure Redis Cache (optional - commented out for SQLite simplicity)
// builder.Services.AddStackExchangeRedisCache(options =>
// {
//     options.Configuration = builder.Configuration.GetConnectionString("Redis");
// });

// ========== Dependency Injection Configuration ==========
// Register cache service - can switch between Memory and Redis implementations
builder.Services.AddScoped<ICacheService, MemoryCacheService>();

// Register the StudentService as scoped for proper Entity Framework lifecycle management
// Scoped lifetime ensures a new instance is created for each HTTP request
// This is the recommended pattern when using Entity Framework Core
builder.Services.AddScoped<IStudentService, StudentService>();

// ========== CORS Configuration ==========
// Configure Cross-Origin Resource Sharing (CORS) to allow the Vue.js frontend to communicate with the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        policy =>
        {
            // Allow requests from the Vue development server (Vite default ports)
            policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
                  .AllowAnyHeader()        // Accept any HTTP headers
                  .AllowAnyMethod()        // Accept any HTTP methods (GET, POST, PUT, DELETE, etc.)
                  .AllowCredentials();     // Allow cookies and authentication headers
        });
});

// ========== Logging Configuration ==========
// Clear default logging providers for a clean configuration
builder.Logging.ClearProviders();
// Add console logging for development and debugging
builder.Logging.AddConsole();
// Add debug output logging (visible in IDE debug console)
builder.Logging.AddDebug();

// Build the web application with all configured services
var app = builder.Build();

// ========== Database Seeding ==========
// Seed the database with sample data for development
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StudentDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    await SeedAsync(context, logger);
}

// ========== HTTP Request Pipeline Configuration ==========
// The order of middleware is important - they execute in the order they're added

// Enable Swagger in development environment only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       // Enable Swagger JSON endpoint
    app.UseSwaggerUI();     // Enable Swagger UI at /swagger
}

// Enable CORS with the configured policy
// This must come before UseAuthorization
app.UseCors("AllowVueApp");

// Enable authorization middleware (even though we're not using authentication in this demo)
// This is included for future extensibility
app.UseAuthorization();

// Map controller routes and endpoints
// This enables the API to route incoming requests to the appropriate controller actions
app.MapControllers();

// Start the web application and begin listening for HTTP requests
// The application will run until manually stopped
app.Run();
