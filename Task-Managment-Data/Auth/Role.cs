using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Task_Managment_Data.Core;

namespace Task_Managment_Data.Auth
{
    [Table("Roles")]
    public class Role : AuditableEntity<int>
    {
        [Required(ErrorMessage = "Role name is required"), MaxLength(50)]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}