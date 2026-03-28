using Microsoft.EntityFrameworkCore;
using Task_Managment_Data.Auth;
using Task_Managment_Data.Features;
using TaskStatus = Task_Managment_Data.Features.TaskStatus;

namespace Task_Managment_Data.Core
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.Migrate();

            SeedTaskStatuses(context);
            SeedDepartments(context);
            SeedEmployees(context);
            SeedRolesAndPermissions(context);
            SeedUsers(context);
            SeedTasks(context);
        }

        // -------------------------------------------------------------------------
        // Task Statuses
        // -------------------------------------------------------------------------
        private static void SeedTaskStatuses(AppDbContext context)
        {
            if (context.TaskStatuses.Any()) return;

            context.TaskStatuses.AddRange(
                new TaskStatus { Id = (int)TaskStatusEnum.Pending, Name = "Pending" },
                new TaskStatus { Id = (int)TaskStatusEnum.InProgress, Name = "In Progress" },
                new TaskStatus { Id = (int)TaskStatusEnum.Completed, Name = "Completed" },
                new TaskStatus { Id = (int)TaskStatusEnum.Cancelled, Name = "Cancelled" }
            );
            context.SaveChanges();
        }

        // -------------------------------------------------------------------------
        // Departments
        // -------------------------------------------------------------------------
        private static void SeedDepartments(AppDbContext context)
        {
            if (context.Departments.Any()) return;

            context.Departments.AddRange(
                new Department { Name = "Human Resources" },
                new Department { Name = "Operations" },
                new Department { Name = "Logistics" },
                new Department { Name = "IT" },
                new Department { Name = "Procurement" }
            );
            context.SaveChanges();
        }

        // -------------------------------------------------------------------------
        // Employees 
        // -------------------------------------------------------------------------
        private static void SeedEmployees(AppDbContext context)
        {
            if (context.Employees.Any()) return;

            var hr = context.Departments.First(d => d.Name == "Human Resources");
            var operations = context.Departments.First(d => d.Name == "Operations");
            var logistics = context.Departments.First(d => d.Name == "Logistics");
            var it = context.Departments.First(d => d.Name == "IT");
            var procurement = context.Departments.First(d => d.Name == "Procurement");

            context.Employees.AddRange(

                // Human Resources
                new Employee { FirstName = "Alice", LastName = "Johnson", JobTitle = "HR Manager", DepartmentId = hr.Id },
                new Employee { FirstName = "Jon", LastName = "Micle", JobTitle = "HR Specialist", DepartmentId = hr.Id },
                new Employee { FirstName = "Doe", LastName = "Alice", JobTitle = "HR Specialist", DepartmentId = hr.Id },

                // IT
                new Employee { FirstName = "Bob", LastName = "Smith", JobTitle = "Senior Engineer", DepartmentId = it.Id },
                new Employee { FirstName = "Carol", LastName = "Williams", JobTitle = "Software Engineer", DepartmentId = it.Id },
                new Employee { FirstName = "Eve", LastName = "Davis", JobTitle = "IT Support Specialist", DepartmentId = it.Id },

                // Logistics
                new Employee { FirstName = "David", LastName = "Brown", JobTitle = "Logistics Administrator", DepartmentId = logistics.Id },
                new Employee { FirstName = "Frank", LastName = "Miller", JobTitle = "Logistics Coordinator", DepartmentId = logistics.Id },
                new Employee { FirstName = "Grace", LastName = "Wilson", JobTitle = "Logistics Coordinator", DepartmentId = logistics.Id },

                // Procurement
                new Employee { FirstName = "Emma", LastName = "Clark", JobTitle = "Procurement Specialist", DepartmentId = procurement.Id },
                new Employee { FirstName = "Charlie", LastName = "Garcia", JobTitle = "Procurement Specialist", DepartmentId = procurement.Id },
                new Employee { FirstName = "Sophia", LastName = "Lee", JobTitle = "Procurement Specialist", DepartmentId = procurement.Id },

                // Operations 
                new Employee { FirstName = "Nathan", LastName = "Brown", JobTitle = "Operations Manager", DepartmentId = operations.Id },
                new Employee { FirstName = "Henry", LastName = "Miller", JobTitle = "Operations Coordinator", DepartmentId = operations.Id },
                new Employee { FirstName = "Jon", LastName = "Miller", JobTitle = "Operations Coordinator", DepartmentId = operations.Id }
            );
            context.SaveChanges();
        }

        private static void SeedRolesAndPermissions(AppDbContext context)
        {
            if (context.Roles.Any()) return;

            var permissions = new List<Permission>
            {
                new Permission { Name = "Task.Create"   },
                new Permission { Name = "Task.Read"     },
                new Permission { Name = "Task.Update"   },
                new Permission { Name = "Task.Delete"   },
                new Permission { Name = "Task.Complete" },
                new Permission { Name = "User.Manage"   }
            };
            context.Permissions.AddRange(permissions);
            context.SaveChanges();

            var adminRole = new Role { Name = "Admin", Description = "Full system access" };
            var managerRole = new Role { Name = "Manager", Description = "Manage task completion and staff" };
            var employeeRole = new Role { Name = "Employee", Description = "View and update own tasks" };

            context.Roles.AddRange(adminRole, managerRole, employeeRole);
            context.SaveChanges();

            var allPermissions = context.Permissions.ToList();
            Permission Perm(string name) => allPermissions.First(p => p.Name == name);

            context.RolePermissions.AddRange(

                // Admin — all permissions
                new RolePermission { RoleId = adminRole.Id, PermissionId = Perm("Task.Create").Id },
                new RolePermission { RoleId = adminRole.Id, PermissionId = Perm("Task.Read").Id },
                new RolePermission { RoleId = adminRole.Id, PermissionId = Perm("Task.Update").Id },
                new RolePermission { RoleId = adminRole.Id, PermissionId = Perm("Task.Delete").Id },
                new RolePermission { RoleId = adminRole.Id, PermissionId = Perm("Task.Complete").Id },
                new RolePermission { RoleId = adminRole.Id, PermissionId = Perm("User.Manage").Id },

                // Manager — everything except User.Manage
                new RolePermission { RoleId = managerRole.Id, PermissionId = Perm("Task.Create").Id },
                new RolePermission { RoleId = managerRole.Id, PermissionId = Perm("Task.Read").Id },
                new RolePermission { RoleId = managerRole.Id, PermissionId = Perm("Task.Update").Id },
                new RolePermission { RoleId = managerRole.Id, PermissionId = Perm("Task.Delete").Id },
                new RolePermission { RoleId = managerRole.Id, PermissionId = Perm("Task.Complete").Id },

                // Employee — create, read, update, complete own tasks
                new RolePermission { RoleId = employeeRole.Id, PermissionId = Perm("Task.Create").Id },
                new RolePermission { RoleId = employeeRole.Id, PermissionId = Perm("Task.Read").Id },
                new RolePermission { RoleId = employeeRole.Id, PermissionId = Perm("Task.Update").Id },
                new RolePermission { RoleId = employeeRole.Id, PermissionId = Perm("Task.Complete").Id }
            );
            context.SaveChanges();
        }

        private static void SeedUsers(AppDbContext context)
        {
            if (context.Users.Any()) return;

            var alice = context.Employees.First(e => e.FirstName == "Alice" && e.LastName == "Johnson");
            var bob = context.Employees.First(e => e.FirstName == "Bob" && e.LastName == "Smith");
            var carol = context.Employees.First(e => e.FirstName == "Carol" && e.LastName == "Williams");
            var david = context.Employees.First(e => e.FirstName == "David" && e.LastName == "Brown");

            var adminRole = context.Roles.First(r => r.Name == "Admin");
            var managerRole = context.Roles.First(r => r.Name == "Manager");
            var employeeRole = context.Roles.First(r => r.Name == "Employee");

            var users = new List<User>
            {
                new User
                {
                    UserName     = "alice.johnson",
                    PasswordHash = HashPassword("Admin@123"),
                    Email        = "alice.johnson@company.com",
                    EmployeeId   = alice.Id,
                    UserRoles    = new List<UserRole>()
                },
                new User
                {
                    UserName     = "bob.smith",
                    PasswordHash = HashPassword("Manager@123"),
                    Email        = "bob.smith@company.com",
                    EmployeeId   = bob.Id,
                    UserRoles    = new List<UserRole>()
                },
                new User
                {
                    UserName     = "carol.williams",
                    PasswordHash = HashPassword("Employee@123"),
                    Email        = "carol.williams@company.com",
                    EmployeeId   = carol.Id,
                    UserRoles    = new List<UserRole>()
                },
                new User
                {
                    UserName     = "david.brown",
                    PasswordHash = HashPassword("Employee@123"),
                    Email        = "david.brown@company.com",
                    EmployeeId   = david.Id,
                    UserRoles    = new List<UserRole>()
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            var aliceUser = context.Users.First(u => u.UserName == "alice.johnson");
            var bobUser = context.Users.First(u => u.UserName == "bob.smith");
            var carolUser = context.Users.First(u => u.UserName == "carol.williams");
            var davidUser = context.Users.First(u => u.UserName == "david.brown");

            context.UserRoles.AddRange(
                new UserRole { UserId = aliceUser.Id, RoleId = adminRole.Id },
                new UserRole { UserId = bobUser.Id, RoleId = managerRole.Id },
                new UserRole { UserId = carolUser.Id, RoleId = employeeRole.Id }, 
                new UserRole { UserId = davidUser.Id, RoleId = employeeRole.Id }
            );
            context.SaveChanges();
        }

        // -------------------------------------------------------------------------
        // Tasks
        // -------------------------------------------------------------------------
        private static void SeedTasks(AppDbContext context)
        {
            if (context.Tasks.Any()) return;

            var bob = context.Employees.First(e => e.FirstName == "Bob" && e.LastName == "Smith");
            var carol = context.Employees.First(e => e.FirstName == "Carol" && e.LastName == "Williams");
            var david = context.Employees.First(e => e.FirstName == "David" && e.LastName == "Brown"); // FIX: was ambiguous

            var it = context.Departments.First(d => d.Name == "IT");
            var logistics = context.Departments.First(d => d.Name == "Logistics");
            var hr = context.Departments.First(d => d.Name == "Human Resources");
            var procurement = context.Departments.First(d => d.Name == "Procurement");

            context.Tasks.AddRange(
                new WorkTask
                {
                    Title = "Set up CI/CD pipeline",
                    Description = "Configure GitHub Actions for build and deployment",
                    AssignedToId = bob.Id,
                    DepartmentId = it.Id,
                    TaskStatusId = (int)TaskStatusEnum.InProgress,
                    CreatedBy = "alice.johnson"
                },
                new WorkTask
                {
                    Title = "Implement authentication module",
                    Description = "JWT-based login and role validation",
                    AssignedToId = carol.Id,
                    DepartmentId = it.Id,
                    TaskStatusId = (int)TaskStatusEnum.Pending,
                    CreatedBy = "alice.johnson"
                },
                new WorkTask
                {
                    Title = "Prepare Q3 procurement report",
                    Description = "Compile regional procurement figures for Q3",
                    AssignedToId = david.Id,
                    DepartmentId = procurement.Id,
                    TaskStatusId = (int)TaskStatusEnum.Pending,
                    CreatedBy = "bob.smith"
                },
                new WorkTask
                {
                    Title = "Code review — payment service",
                    Description = "Review and approve payment service PR",
                    AssignedToId = bob.Id,
                    DepartmentId = it.Id,
                    TaskStatusId = (int)TaskStatusEnum.Completed,
                    CreatedBy = "alice.johnson"
                },
                new WorkTask
                {
                    Title = "Migrate legacy database to SQL Server",
                    Description = "Export data from the old MySQL database, transform schemas, and import into the new SQL Server instance with full data validation",
                    AssignedToId = bob.Id,
                    DepartmentId = it.Id,
                    TaskStatusId = (int)TaskStatusEnum.Pending,
                    CreatedAt = new DateTime(2026, 3, 20, 9, 0, 0, DateTimeKind.Utc),
                    CreatedBy = "alice.johnson",
                    UpdatedAt = new DateTime(2026, 3, 22, 14, 30, 0, DateTimeKind.Utc),
                    UpdatedBy = "bob.smith"
                },
                new WorkTask
                {
                    Title = "Conduct annual employee satisfaction survey",
                    Description = "Design the survey questionnaire, distribute to all departments, collect responses, and compile a summary report for management review",
                    AssignedToId = carol.Id,
                    DepartmentId = hr.Id,
                    TaskStatusId = (int)TaskStatusEnum.Completed,
                    CreatedAt = new DateTime(2026, 2, 10, 8, 0, 0, DateTimeKind.Utc),
                    CreatedBy = "bob.smith",
                    UpdatedAt = new DateTime(2026, 3, 15, 16, 45, 0, DateTimeKind.Utc),
                    UpdatedBy = "carol.williams"
                },
                new WorkTask
                {
                    Title = "Evaluate new office supply vendors",
                    Description = "Research and compare at least five vendors for office supplies, negotiate pricing, and present a recommendation to the procurement lead",
                    AssignedToId = david.Id,
                    DepartmentId = procurement.Id,
                    TaskStatusId = (int)TaskStatusEnum.Cancelled,
                    CreatedAt = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc),
                    CreatedBy = "alice.johnson",
                    UpdatedAt = new DateTime(2026, 2, 28, 11, 20, 0, DateTimeKind.Utc),
                    UpdatedBy = "alice.johnson"
                }
            );
            context.SaveChanges();
        }

        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        private static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}