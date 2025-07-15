# Student Management System

A full-stack web application for managing student records, built with Vue 3 (frontend) and ASP.NET Core (backend).

## Project Structure

### Phase 1: Client-Side (Completed)
- **Framework**: Vue 3 with TypeScript
- **Build Tool**: Vite
- **Features**:
  - Modern, beautiful UI with gradient backgrounds and animations
  - Student list display with enhanced visual design
  - API service layer with Axios
  - Loading states with modern spinner animations
  - Error handling with improved visual feedback
  - Responsive design with mobile optimization
  - Comprehensive logging system with dual-tag structure (feature/module)

### Components

#### StudentListSimple.vue
Main component displaying students in a beautifully designed table interface. This component is now the active implementation in App.vue, featuring a modern design with enhanced visual appeal.

**Design Features:**
- Modern gradient backgrounds and visual effects
- Enhanced table styling with hover animations
- Beautiful loading states with multi-ring spinner
- Improved error and empty state designs
- Status badges with gradient styling
- Responsive design for mobile devices
- Professional typography and spacing

#### Logger System
- Dual-tag logging (feature tag + module tag)
- Tracks function names, parameters, and timestamps
- Supports sorting by feature or module
- Different log levels (info, warning, error, debug)

### Running the Client

```bash
cd student-management-client
npm install
npm run dev
```

The client will run on `http://localhost:5173` by default.

### API Configuration

The client expects the backend API to be running on `http://localhost:5002/api`. This can be configured in the `.env` file.

## Student Model

```typescript
interface Student {
  studentId: number;
  fullName: string;
  birthDate: string;
  averageGrade: number;
  isActive: boolean;
}
```

## Phase 2: Server-Side (Completed)
- **Framework**: ASP.NET Core Web API
- **Architecture**: Controller-Service pattern with separation of concerns
- **Features**:
  - Full CRUD operations for student management
  - Model validation with data annotations
  - Proper HTTP status codes (200, 201, 204, 400, 404, 500)
  - CORS configuration for Vue frontend
  - Comprehensive logging with feature/module tags
  - Entity Framework Core with SQLite database integration

### API Endpoints

- `GET /api/students` - Get all students
- `GET /api/students/{id}` - Get student by ID
- `POST /api/students` - Create new student
- `PUT /api/students/{id}` - Update existing student
- `DELETE /api/students/{id}` - Delete student

### Running the Server

```bash
cd StudentManagementAPI
dotnet run
```

The API will run on `http://localhost:5002` with Swagger UI available at `http://localhost:5002/swagger`.

## Testing the Application

1. Start the backend server:
   ```bash
   cd StudentManagementAPI
   dotnet run
   ```

2. In a new terminal, start the Vue client:
   ```bash
   cd student-management-client
   npm run dev
   ```

3. Open your browser to `http://localhost:5173` to view the student list.

4. Use Swagger UI at `http://localhost:5000/swagger` to test CRUD operations.

## Phase 3: Database Integration (Completed)
- **Database**: SQLite with Entity Framework Core
- **Architecture**: Code-first approach with migrations
- **Features**:
  - StudentDbContext with proper entity configuration
  - Database migrations for schema management
  - Automatic database seeding with sample data
  - Optimized indexing for performance
  - Connection string configuration
  - Async database operations

### Database Schema

The SQLite database contains a `Students` table with the following structure:
- `StudentId` (INTEGER, PRIMARY KEY, AUTOINCREMENT)
- `FullName` (TEXT, NOT NULL, MAX 100 characters)
- `BirthDate` (DATE, NOT NULL)
- `AverageGrade` (REAL, NOT NULL, 0-100 range)
- `IsActive` (INTEGER, NOT NULL, DEFAULT 1)

### Database Files

- `students.db` - SQLite database file (created automatically)
- `Migrations/` - Entity Framework migration files

## Project Complete

All three phases have been successfully implemented:
1. ✅ **Phase 1**: Vue 3 frontend with virtual scrolling and API integration
2. ✅ **Phase 2**: ASP.NET Core Web API with full CRUD operations
3. ✅ **Phase 3**: SQLite database with Entity Framework Core