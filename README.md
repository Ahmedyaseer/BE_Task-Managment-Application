# AKG Task Management System

A full-stack web application for recording and tracking employee tasks across five departments (HR, Operations, Logistics, IT, Procurement) with JWT authentication and role-based access control.

## What This Project Does

This is a complete task management system with an ASP.NET Core REST API backend and an Angular SPA frontend that handles:

- **Task Management** - Create, edit, complete, and delete tasks with status tracking
- **Employee Management** - Manage employees across multiple departments
- **Department Management** - Organize the workforce into departments
- **User & Role Management** - Create users, assign roles, and configure permissions
- **JWT Authentication** - Secure login with role-based access (Admin/Manager/Employee)
- **Advanced Filtering** - Search, filter, sort, and paginate all data with dynamic queries

The system follows a realistic business workflow where tasks flow through statuses (Pending, In Progress, Completed, Cancelled) and access is controlled through a permission-based authorization system.

## Key Features

- **JWT Authentication** - Secure token-based login with permission claims
- **Role-Based Access Control** - Three roles (Admin, Manager, Employee) with configurable permissions
- **Generic Filter System** - One unified filter DTO for all list endpoints with global search, field filters, sorting, and pagination
- **Auto Database Setup** - Migrations and seed data run automatically on first startup
- **Unit of Work Pattern** - Clean data access through `IUnitOfWork` and `IBaseRepository<T>`
- **Angular SPA** - Standalone components, lazy-loaded routes, Bootstrap 5 UI

## Technology Stack

- **C# / .NET 10** with **ASP.NET Core Web API**
- **SQL Server Express** with **Entity Framework Core**
- **Angular 17** with **TypeScript** (Standalone Components)
- **Bootstrap 5** for styling
- **JWT Bearer** for authentication
- **EF Core Migrations** for database versioning

## Quick Start

### Prerequisites

- .NET SDK 10.0+
- Node.js 18+
- SQL Server Express (any recent version)
- Angular CLI 17+ (`npm install -g @angular/cli`)

### Setup

1. **Clone and navigate to project**

```bash
git clone <repository-url>
cd AKG
```

2. **Verify SQL Server Express is running**

Open **Services** (services.msc) and make sure `SQL Server (SQLEXPRESS)` is started. The app connects using Windows Authentication — no username/password needed.

3. **Build and run the backend**

```bash
dotnet build AKG-Task-Managment-System/AKG-Task-Managment-System.slnx
dotnet run --project AKG-Task-Managment-System/AKG-Task-Managment-System
```

The API starts at **http://localhost:5159**

On first run, the app automatically creates the database, applies migrations, and seeds all test data.

4. **Install and run the frontend**

```bash
cd TMA-Frontend
npm install
npx ng serve
```

The frontend starts at **http://localhost:4200**

> **Important:** The backend must be running first — the frontend connects to `http://localhost:5159/api`.

## Default Logins

| Username | Password | Role | Access Level |
|----------|----------|------|-------------|
| `alice.johnson` | `Admin@123` | Admin | Full access — tasks, employees, departments, users, roles |
| `bob.smith` | `Manager@123` | Manager | Task CRUD + delete + complete |
| `carol.williams` | `Employee@123` | Employee | Task create, read, update, complete |
| `david.brown` | `Employee@123` | Employee | Task create, read, update, complete |

## How to Test the API

### 1. Get Authentication Token

```bash
curl -X POST http://localhost:5159/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"userName": "alice.johnson", "password": "Admin@123"}'
```

Response:
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "userName": "alice.johnson",
    "role": "Admin",
    "permissions": ["Task.Create", "Task.Read", "Task.Update", "Task.Delete", "Task.Complete", "User.Manage"],
    "expiresInMinutes": 60
  }
}
```

### 2. Create a Task

```bash
curl -X POST http://localhost:5159/api/tasks \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Deploy new release",
    "description": "Deploy v2.1 to production servers",
    "assignedToId": 4
  }'
```

### 3. Complete a Task

```bash
curl -X PUT http://localhost:5159/api/tasks/1/complete \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### 4. Filter Tasks by Status

```bash
curl -X POST http://localhost:5159/api/tasks/filter \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "page": 0,
    "size": 10,
    "sortBy": "createdAt",
    "sortDirection": "desc",
    "filters": [
      { "field": "taskStatusId", "operator": "EQUALS", "value": "1" }
    ]
  }'
```

### 5. Global Search

```bash
curl -X POST http://localhost:5159/api/tasks/filter \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "page": 0,
    "size": 10,
    "globalSearch": "database"
  }'
```

## API Endpoints

All endpoints (except login) require a JWT token in the `Authorization: Bearer <token>` header.

### Authentication

- `POST /api/auth/login` - Login and get JWT token

### Tasks

- `POST /api/tasks` - Create task (requires `Task.Create`)
- `GET /api/tasks/{id}` - Get task by ID (requires `Task.Read`)
- `PUT /api/tasks/{id}` - Update task (requires `Task.Update`)
- `PUT /api/tasks/{id}/complete` - Complete task (requires `Task.Complete`)
- `DELETE /api/tasks/{id}` - Delete task (requires `Task.Delete`)
- `POST /api/tasks/filter` - Filtered/paginated list (requires `Task.Read`)

### Employees

- `GET /api/employees` - List all employees (authenticated)
- `POST /api/employees` - Create employee (requires `User.Manage`)
- `PUT /api/employees/{id}` - Update employee (requires `User.Manage`)
- `DELETE /api/employees/{id}` - Delete employee (requires `User.Manage`)
- `POST /api/employees/filter` - Filtered/paginated list (authenticated)

### Departments

- `GET /api/departments` - List all departments (requires `User.Manage`)
- `POST /api/departments` - Create department (requires `User.Manage`)
- `PUT /api/departments/{id}` - Update department (requires `User.Manage`)
- `DELETE /api/departments/{id}` - Delete department (requires `User.Manage`)
- `POST /api/departments/filter` - Filtered/paginated list (requires `User.Manage`)

### Users

- `GET /api/users` - List all users (requires `User.Manage`)
- `POST /api/users` - Create user (requires `User.Manage`)
- `PUT /api/users/{id}` - Update user (requires `User.Manage`)
- `DELETE /api/users/{id}` - Delete user (requires `User.Manage`)
- `POST /api/users/filter` - Filtered/paginated list (requires `User.Manage`)

### Roles & Permissions

- `GET /api/roles` - List roles with permissions (requires `User.Manage`)
- `GET /api/permissions` - List all permissions (requires `User.Manage`)
- `PUT /api/roles/{id}/permissions` - Update role permissions (requires `User.Manage`)

### Filtering

All `/filter` endpoints support the same `GeneralFilterDto` body with operators: `EQUALS`, `CONTAINS`.

## Task Workflow

```
PENDING → IN_PROGRESS → COMPLETED
   ↓
CANCELLED
```

- New tasks start as **Pending**
- Tasks can be updated to any status via edit
- The **Complete** action sets status to Completed (blocked if already completed or cancelled)
- Admin and Manager roles can **Delete** tasks

## Architecture Highlights

### Three-Layer Solution

```
AKG-Task-Managment-System (Web API)  ──>  TMA-Core + Task-Managment-Data
Task-Managment-Data (Data layer)     ──>  TMA-Core
TMA-Core (Interfaces only)
```

- **TMA-Core** - Repository interfaces (`IBaseRepository<T>`, `IUnitOfWork`). Zero dependencies.
- **Task-Managment-Data** - EF Core implementation: entities, `AppDbContext`, repositories, migrations, seed data.
- **AKG-Task-Managment-System** - Web API host: controllers, DTOs, services, JWT configuration.

### Permission System

```
Users ──M:M── Roles ──M:M── Permissions
```

Permissions are embedded as JWT claims. The API uses policy-based authorization:

| Policy | Permission Claim | Used By |
|--------|-----------------|---------|
| CanCreateTask | `Task.Create` | POST /api/tasks |
| CanReadTask | `Task.Read` | GET/POST filter |
| CanUpdateTask | `Task.Update` | PUT /api/tasks/{id} |
| CanDeleteTask | `Task.Delete` | DELETE /api/tasks/{id} |
| CanCompleteTask | `Task.Complete` | PUT /api/tasks/{id}/complete |
| CanManageUsers | `User.Manage` | All employee/department/user/role endpoints |

### Response Format

All API responses follow a consistent envelope:

```json
{
  "success": true,
  "statusCode": 200,
  "message": "Success",
  "data": { ... }
}
```

Error responses:
```json
{
  "success": false,
  "statusCode": 400,
  "message": "Task not found",
  "data": null
}
```

## System Diagrams

- **Entity Relationship Diagram**: [AKG-Task-Management-ERD.pdf](./AKG-Task-Management-ERD.pdf)
- **Authentication Flow**: [Sequence-1-Authentication.pdf](./Sequence-1-Authentication.pdf) - Login, JWT token generation
- **Task Creation Flow**: [Sequence-2-Task-Creation.pdf](./Sequence-2-Task-Creation.pdf) - Form submission through to database insert
- **Task Completion Flow**: [Sequence-3-Task-Completion.pdf](./Sequence-3-Task-Completion.pdf) - Status validation and update
- **Task Filter Flow**: [Sequence-4-Task-Filter.pdf](./Sequence-4-Task-Filter.pdf) - Dynamic query building, pagination

## Project Structure

### Backend

```
AKG-Task-Managment-System/
├── AKG-Task-Managment-System/         # Web API Project
│   ├── Controllers/                   # API endpoints
│   ├── Services/                      # Business logic
│   ├── DTOs/                          # Request/response models
│   │   ├── Auth/                      # Login, Role, User DTOs
│   │   ├── Common/                    # GeneralFilterDto, PagedResponse, ResponseDto
│   │   ├── Employee/                  # Employee & Department DTOs
│   │   └── Task/                      # Task DTOs
│   └── Program.cs                     # App configuration & middleware
├── Task-Managment-Data/               # Data Layer Project
│   ├── Auth/                          # User, Role, Permission, UserRole, RolePermission entities
│   ├── Features/                      # Department, Employee, WorkTask, TaskStatus entities
│   ├── Core/                          # AppDbContext, SeedData, DependencyInjection
│   ├── Repository/                    # BaseRepository<T>, UnitOfWork
│   ├── Enums/                         # TaskStatusEnum
│   └── Migrations/                    # EF Core migrations
└── TMA-Core/                          # Core Interfaces Project
    └── Repository/                    # IBaseRepository<T>, IUnitOfWork
```

### Frontend

```
TMA-Frontend/src/app/
├── core/
│   ├── constants/                     # API base URL
│   ├── guards/                        # Auth & permission guards
│   ├── interceptors/                  # JWT token interceptor
│   ├── models/                        # TypeScript interfaces
│   └── services/                      # API service classes
├── features/
│   ├── auth/                          # Login page
│   ├── dashboard/                     # Stats overview
│   ├── tasks/                         # Task CRUD
│   ├── departments/                   # Department management
│   ├── employees/                     # Employee management
│   ├── users/                         # User management
│   └── roles/                         # Role permission assignment
└── shared/components/                 # Navbar, search filter, confirm dialog
```

## Database

### Connection String

Configured in `AKG-Task-Managment-System/AKG-Task-Managment-System/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=TaskManagementDb;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

If your SQL Server instance name is different, update `Server=` accordingly.

### Entity Relationship Diagram

```
Departments 1──M Employees 1──M Tasks M──1 TaskStatuses
                 Employees 1──1 Users
                                Users M──M Roles       (via UserRoles)
                                           Roles M──M Permissions (via RolePermissions)
```

**9 tables** — All entities (except join tables) inherit audit fields: `Id`, `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`.

### Seed Data

On first startup in Development mode, the app seeds:
- **4 task statuses** (Pending, In Progress, Completed, Cancelled)
- **5 departments** (HR, Operations, Logistics, IT, Procurement)
- **15 employees** across all departments
- **3 roles** (Admin, Manager, Employee) with **6 permissions**
- **4 users** with role assignments
- **7 sample tasks** in various statuses

### Resetting the Database

```sql
-- In SSMS:
DROP DATABASE TaskManagementDb;
```

Then restart the backend — it recreates and re-seeds everything automatically.

### Adding Migrations

```bash
dotnet ef migrations add <MigrationName> \
  --project AKG-Task-Managment-System/Task-Managment-Data \
  --startup-project AKG-Task-Managment-System/AKG-Task-Managment-System
```

Migrations are applied automatically on startup via `context.Database.Migrate()`.

## Troubleshooting

| Problem | Solution |
|---------|----------|
| **"Cannot access file — used by another process"** | Previous API instance is running. Stop it with Ctrl+C or `taskkill /F /IM AKG-Task-Managment-System.exe` |
| **Database connection fails** | Verify SQL Server Express is running in Services (services.msc). Check the instance name in `appsettings.json`. |
| **Frontend shows "Failed to load" errors** | Backend API must be running on `http://localhost:5159` before starting the frontend. |
| **Seed data not appearing** | Seed only runs when tables are empty. Drop the database and restart to re-seed. |
| **Angular CLI not found** | Run `npm install -g @angular/cli` to install globally. |
| **`dotnet ef` not found** | Install the EF tools: `dotnet tool install --global dotnet-ef` |

---

**Built for AK-Group** - This project demonstrates a full-stack task management system with ASP.NET Core, Angular, and SQL Server following clean architecture principles.

**Built by Ahmed Yasser** - This project demonstrates modern ASP.NET Core Web API development practices with clean architecture and real-world business requirements.