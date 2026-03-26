using System.ComponentModel.DataAnnotations;

namespace AKG_Task_Managment_System.DTOs.Auth
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Role { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
        public int ExpiresInMinutes { get; set; }
    }
}
