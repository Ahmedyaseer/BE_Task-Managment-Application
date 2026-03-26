using Microsoft.AspNetCore.Mvc;
using AKG_Task_Managment_System.DTOs.Auth;
using AKG_Task_Managment_System.DTOs.Common;
using AKG_Task_Managment_System.Services;

namespace AKG_Task_Managment_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(AuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await authService.LoginAsync(dto);
                return Ok(ResponseDto<LoginResponseDto>.SuccessResponse(result, "Login successful"));
            }
            catch (Exception ex)
            {
                return Unauthorized(ResponseDto<LoginResponseDto>.ErrorResponse(ex.Message, 401));
            }
        }
    }
}
