using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Managment_Data.Auth
{

    [Table("UserRoles")]
    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;
    }
}