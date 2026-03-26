using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AKG_Task_Managment_System.DTOs.Common;
using AKG_Task_Managment_System.DTOs.Employee;
using AKG_Task_Managment_System.Services;

namespace AKG_Task_Managment_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CanManageUsers")]
    public class DepartmentsController(DepartmentService departmentService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await departmentService.GetAllAsync();
            return Ok(ResponseDto<List<DepartmentDto>>.SuccessResponse(result));
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetFiltered([FromBody] GeneralFilterDto filter)
        {
            var result = await departmentService.GetFilteredAsync(filter);
            return Ok(ResponseDto<PagedResponse<DepartmentDto>>.SuccessResponse(result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentDto dto)
        {
            try
            {
                var result = await departmentService.CreateAsync(dto, User.Identity?.Name);
                return Ok(ResponseDto<DepartmentDto>.SuccessResponse(result, "Department created"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseDto<DepartmentDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentDto dto)
        {
            try
            {
                var result = await departmentService.UpdateAsync(id, dto, User.Identity?.Name);
                return Ok(ResponseDto<DepartmentDto>.SuccessResponse(result, "Department updated"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseDto<DepartmentDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await departmentService.DeleteAsync(id);
                return Ok(ResponseDto<string>.SuccessResponse("Deleted", "Department deleted"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseDto<string>.ErrorResponse(ex.Message));
            }
        }
    }
}
