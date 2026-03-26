using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Task_Managment_Data.Core;

namespace Task_Managment_Data.Auth
{
    [Table("Permissions")]
    public class Permission : AuditableEntity<int>
    {
        [Required(ErrorMessage = "Permission name is required"), MaxLength(50)]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}