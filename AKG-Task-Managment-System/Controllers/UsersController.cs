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
    public class UsersController(UserService userService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await userService.GetAllAsync();
            return Ok(ResponseDto<List<UserResponseDto>>.SuccessResponse(users));
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetFiltered([FromBody] GeneralFilterDto filter)
        {
            var result = await userService.GetFilteredAsync(filter);
            return Ok(ResponseDto<PagedResponse<UserResponseDto>>.SuccessResponse(result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            try
            {
                var user = await userService.CreateAsync(dto, User.Identity?.Name);
                return Ok(ResponseDto<UserResponseDto>.SuccessResponse(user, "User created successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseDto<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                var user = await userService.UpdateAsync(id, dto, User.Identity?.Name);
                return Ok(ResponseDto<UserResponseDto>.SuccessResponse(user, "User updated successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseDto<string>.ErrorResponse(ex.Message, 404));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseDto<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await userService.DeleteAsync(id);
                return Ok(ResponseDto<string>.SuccessResponse("Deleted", "User deleted successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseDto<string>.ErrorResponse(ex.Message, 404));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseDto<string>.ErrorResponse(ex.Message));
            }
        }
    }
}
