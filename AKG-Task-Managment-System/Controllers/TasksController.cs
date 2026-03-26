using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AKG_Task_Managment_System.DTOs.Common;
using AKG_Task_Managment_System.DTOs.Task;
using AKG_Task_Managment_System.Services;

namespace AKG_Task_Managment_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController(TaskService taskService) : ControllerBase
    {
        [HttpDelete("{id}")]
        [Authorize(Policy = "CanDeleteTask")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await taskService.DeleteAsync(id);
                return Ok(ResponseDto<string>.SuccessResponse("Deleted", "Task deleted"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseDto<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Policy = "CanCreateTask")]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
        {
            try
            {
                var result = await taskService.CreateAsync(dto, User.Identity?.Name);
                return Ok(ResponseDto<TaskResponseDto>.SuccessResponse(result, "Task created"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseDto<TaskResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "CanUpdateTask")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
        {
            try
            {
                var result = await taskService.UpdateAsync(id, dto, User.Identity?.Name);
                return Ok(ResponseDto<TaskResponseDto>.SuccessResponse(result, "Task updated"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseDto<TaskResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}/complete")]
        [Authorize(Policy = "CanCompleteTask")]
        public async Task<IActionResult> Complete(int id)
        {
            try
            {
                var result = await taskService.CompleteAsync(id, User.Identity?.Name);
                return Ok(ResponseDto<TaskResponseDto>.SuccessResponse(result, "Task completed"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseDto<TaskResponseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "CanReadTask")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await taskService.GetByIdAsync(id);
                return Ok(ResponseDto<TaskResponseDto>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return NotFound(ResponseDto<TaskResponseDto>.ErrorResponse(ex.Message, 404));
            }
        }

        [HttpPost("filter")]
        [Authorize(Policy = "CanReadTask")]
        public async Task<IActionResult> GetFiltered([FromBody] GeneralFilterDto filter)
        {
            var result = await taskService.GetFilteredAsync(filter);
            return Ok(ResponseDto<PagedResponse<TaskResponseDto>>.SuccessResponse(result));
        }
    }
}
