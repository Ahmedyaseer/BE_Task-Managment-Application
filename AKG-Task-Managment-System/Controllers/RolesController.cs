using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AKG_Task_Managment_System.DTOs.Auth;
using AKG_Task_Managment_System.DTOs.Common;
using AKG_Task_Managment_System.Services;

namespace AKG_Task_Managment_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CanManageUsers")]
    public class RolesController(RoleService roleService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await roleService.GetAllRolesAsync();
            return Ok(ResponseDto<List<RoleResponseDto>>.SuccessResponse(roles));
        }

        [HttpGet("/api/permissions")]
        public async Task<IActionResult> GetPermissions()
        {
            var permissions = await roleService.GetAllPermissionsAsync();
            return Ok(ResponseDto<List<PermissionResponseDto>>.SuccessResponse(permissions));
        }

        [HttpPut("{id}/permissions")]
        public async Task<IActionResult> UpdatePermissions(int id, [FromBody] UpdateRolePermissionsDto dto)
        {
            try
            {
                await roleService.UpdateRolePermissionsAsync(id, dto.PermissionIds);
                return Ok(ResponseDto<string>.SuccessResponse("Updated", "Permissions updated"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseDto<string>.ErrorResponse(ex.Message));
            }
        }
    }
}
