using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Task_Managment_Data.Core;
using Task_Managment_Data.Features;

namespace Task_Managment_Data.Auth
{
    [Table("Users")]
    public class User : AuditableEntity<int>
    {
        [Required(ErrorMessage = "User name is required"), MaxLength(50)]
        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [JsonPropertyName("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address"), MaxLength(100)]
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}