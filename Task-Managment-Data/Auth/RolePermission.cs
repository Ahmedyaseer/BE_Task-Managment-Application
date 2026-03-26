using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Managment_Data.Auth
{
    [Table("RolePermissions")]
    public class RolePermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;

        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; } = null!;
    }
}