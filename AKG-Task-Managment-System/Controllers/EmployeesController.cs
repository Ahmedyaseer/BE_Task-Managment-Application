using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AKG_Task_Managment_System.DTOs.Common;
using AKG_Task_Managment_System.DTOs.Employee;
using AKG_Task_Managment_System.Services;

namespace AKG_Task_Managment_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController(EmployeeService employeeService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await employeeService.GetAllAsync();
            return Ok(ResponseDto<List<EmployeeResponseDto>>.SuccessResponse(employees));
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetFiltered([FromBody] GeneralFilterDto filter)
        {
            var result = await employeeService.GetFilteredAsync(filter);
            return Ok(ResponseDto<PagedResponse<EmployeeResponseDto>>.SuccessResponse(result));
        }

        [HttpPost]
        [Authorize(Policy = "CanManageUsers")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
        {
            try
            {
                var employee = await employeeService.CreateAsync(dto, User.Identity?.Name);
                return Ok(ResponseDto<EmployeeResponseDto>.SuccessResponse(employee, "Employee created successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseDto<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "CanManageUsers")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
        {
            try
            {
                var employee = await employeeService.UpdateAsync(id, dto, User.Identity?.Name);
                return Ok(ResponseDto<EmployeeResponseDto>.SuccessResponse(employee, "Employee updated successfully."));
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
        [Authorize(Policy = "CanManageUsers")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await employeeService.DeleteAsync(id);
                return Ok(ResponseDto<string>.SuccessResponse("Deleted", "Employee deleted successfully."));
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
