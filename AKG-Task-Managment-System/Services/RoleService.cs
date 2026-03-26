using Microsoft.EntityFrameworkCore;
using AKG_Task_Managment_System.DTOs.Auth;
using Task_Managment_Data.Auth;
using TMA_Core.Repository;

namespace AKG_Task_Managment_System.Services
{
    public class RoleService(IUnitOfWork unitOfWork)
    {
        public async Task<List<RoleResponseDto>> GetAllRolesAsync()
        {
            var roles = await unitOfWork.Repository<Role>().GetQueryable()
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .ToListAsync();

            return roles.Select(r => new RoleResponseDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Permissions = r.RolePermissions.Select(rp => rp.Permission.Name).ToList()
            }).ToList();
        }

        public async Task<List<PermissionResponseDto>> GetAllPermissionsAsync()
        {
            var permissions = await unitOfWork.Repository<Permission>().GetAllAsync();
            return permissions.Select(p => new PermissionResponseDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();
        }

        public async Task UpdateRolePermissionsAsync(int roleId, List<int> permissionIds)
        {
            var role = await unitOfWork.Repository<Role>().GetQueryable()
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == roleId)
                ?? throw new Exception("Role not found");

            role.RolePermissions.Clear();

            foreach (var permId in permissionIds)
            {
                role.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permId
                });
            }

            await unitOfWork.SaveChangesAsync();
        }
    }
}
