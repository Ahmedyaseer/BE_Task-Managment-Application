namespace AKG_Task_Managment_System.DTOs.Auth
{
    public class RoleResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<string> Permissions { get; set; } = new();
    }

    public class PermissionResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateRolePermissionsDto
    {
        public List<int> PermissionIds { get; set; } = new();
    }
}
