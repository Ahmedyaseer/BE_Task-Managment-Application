using System.ComponentModel.DataAnnotations;

namespace AKG_Task_Managment_System.DTOs.Auth
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }

    public class CreateUserDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? Email { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int RoleId { get; set; }
    }

    public class UpdateUserDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? RoleId { get; set; }
    }
}
