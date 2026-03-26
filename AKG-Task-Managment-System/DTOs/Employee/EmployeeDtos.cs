using System.ComponentModel.DataAnnotations;

namespace AKG_Task_Managment_System.DTOs.Employee
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }

    public class CreateEmployeeDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? JobTitle { get; set; }

        [Required]
        public int DepartmentId { get; set; }
    }

    public class UpdateEmployeeDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? JobTitle { get; set; }
        public int? DepartmentId { get; set; }
    }

    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CreateDepartmentDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateDepartmentDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }
    }
}
