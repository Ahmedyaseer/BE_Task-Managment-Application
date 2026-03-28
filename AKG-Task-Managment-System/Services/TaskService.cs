using Microsoft.EntityFrameworkCore;
using AKG_Task_Managment_System.DTOs.Common;
using AKG_Task_Managment_System.DTOs.Task;
using Task_Managment_Data.Features;
using TMA_Core.Repository;

namespace AKG_Task_Managment_System.Services
{
    public class TaskService(IUnitOfWork unitOfWork)
    {
        public async Task<TaskResponseDto> CreateAsync(CreateTaskDto dto, string? userName)
        {
            var employeeExists = await unitOfWork.Repository<Employee>()
                .ExistsAsync(e => e.Id == dto.AssignedToId);

            if (!employeeExists)
                throw new Exception("Employee not found");

            var departmentExists = await unitOfWork.Repository<Department>()
                .ExistsAsync(d => d.Id == dto.DepartmentId);

            if (!departmentExists)
                throw new Exception("Department not found");

            var task = new WorkTask
            {
                Title = dto.Title,
                Description = dto.Description,
                AssignedToId = dto.AssignedToId,
                DepartmentId = dto.DepartmentId,
                TaskStatusId = (int)TaskStatusEnum.Pending,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userName
            };

            await unitOfWork.Repository<WorkTask>().AddAsync(task);
            await unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(task.Id);
        }

        public async Task<TaskResponseDto> UpdateAsync(int id, UpdateTaskDto dto, string? userName)
        {
            var task = await unitOfWork.Repository<WorkTask>().GetByIdAsync(id)
                ?? throw new Exception("Task not found");

            if (dto.Title is not null) task.Title = dto.Title;
            if (dto.Description is not null) task.Description = dto.Description;
            if (dto.AssignedToId.HasValue) task.AssignedToId = dto.AssignedToId.Value;
            if (dto.TaskStatusId.HasValue) task.TaskStatusId = dto.TaskStatusId.Value;
            if (dto.DepartmentId.HasValue) task.DepartmentId = dto.DepartmentId.Value;
            task.UpdatedAt = DateTime.UtcNow;
            task.UpdatedBy = userName;

            unitOfWork.Repository<WorkTask>().Update(task);
            await unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(task.Id);
        }

        public async Task<TaskResponseDto> CompleteAsync(int id, string? userName)
        {
            var task = await unitOfWork.Repository<WorkTask>().GetByIdAsync(id)
                ?? throw new Exception("Task not found");

            if (task.TaskStatusId == (int)TaskStatusEnum.Completed)
                throw new Exception("Task is already completed");
            if (task.TaskStatusId == (int)TaskStatusEnum.Cancelled)
                throw new Exception("Cannot complete a cancelled task");

            task.TaskStatusId = (int)TaskStatusEnum.Completed;
            task.UpdatedAt = DateTime.UtcNow;
            task.UpdatedBy = userName;

            unitOfWork.Repository<WorkTask>().Update(task);
            await unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(task.Id);
        }

        public async Task<TaskResponseDto> GetByIdAsync(int id)
        {
            var task = await unitOfWork.Repository<WorkTask>().GetQueryable()
                .Include(t => t.AssignedTo)
                .Include(t => t.Status)
                .Include(t => t.Department)
                .FirstOrDefaultAsync(t => t.Id == id)
                ?? throw new Exception("Task not found");

            return MapToDto(task);
        }

        public async Task<PagedResponse<TaskResponseDto>> GetFilteredAsync(GeneralFilterDto filter)
        {
            IQueryable<WorkTask> query = unitOfWork.Repository<WorkTask>().GetQueryable()
                .Include(t => t.AssignedTo)
                .Include(t => t.Status)
                .Include(t => t.Department);

            if (!string.IsNullOrWhiteSpace(filter.GlobalSearch))
            {
                var search = filter.GlobalSearch.ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(search) ||
                    (t.Description != null && t.Description.ToLower().Contains(search)));
            }

            foreach (var f in filter.Filters)
            {
                query = (f.Field.ToLower(), f.Operator.ToUpper()) switch
                {
                    ("status" or "taskstatusid", "EQUALS") when int.TryParse(f.Value, out var statusId)
                        => query.Where(t => t.TaskStatusId == statusId),
                    ("assignedtoid", "EQUALS") when int.TryParse(f.Value, out var empId)
                        => query.Where(t => t.AssignedToId == empId),
                    ("departmentid", "EQUALS") when int.TryParse(f.Value, out var deptId)
                        => query.Where(t => t.DepartmentId == deptId),
                    ("title", "CONTAINS")
                        => query.Where(t => t.Title.ToLower().Contains(f.Value.ToLower())),
                    _ => query
                };
            }

            var totalElements = await query.CountAsync();

            query = filter.SortBy.ToLower() switch
            {
                "title" => filter.SortDirection.ToLower() == "asc"
                    ? query.OrderBy(t => t.Title)
                    : query.OrderByDescending(t => t.Title),
                "createdat" => filter.SortDirection.ToLower() == "asc"
                    ? query.OrderBy(t => t.CreatedAt)
                    : query.OrderByDescending(t => t.CreatedAt),
                "status" or "taskstatusid" => filter.SortDirection.ToLower() == "asc"
                    ? query.OrderBy(t => t.TaskStatusId)
                    : query.OrderByDescending(t => t.TaskStatusId),
                "department" or "departmentid" => filter.SortDirection.ToLower() == "asc"
                    ? query.OrderBy(t => t.Department.Name)
                    : query.OrderByDescending(t => t.Department.Name),
                _ => filter.SortDirection.ToLower() == "asc"
                    ? query.OrderBy(t => t.Id)
                    : query.OrderByDescending(t => t.Id)
            };

            var items = await query
                .Skip(filter.Page * filter.Size)
                .Take(filter.Size)
                .ToListAsync();

            return new PagedResponse<TaskResponseDto>
            {
                Content = items.Select(MapToDto).ToList(),
                TotalElements = totalElements,
                TotalPages = (int)Math.Ceiling(totalElements / (double)filter.Size),
                Size = filter.Size,
                Page = filter.Page
            };
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var task = await unitOfWork.Repository<WorkTask>().GetByIdAsync(id)
                ?? throw new Exception("Task not found");

            unitOfWork.Repository<WorkTask>().Delete(task);
            await unitOfWork.SaveChangesAsync();
        }

        private static TaskResponseDto MapToDto(WorkTask t) => new()
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            AssignedToId = t.AssignedToId,
            AssignedToName = t.AssignedTo != null
                ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : "",
            TaskStatusId = t.TaskStatusId,
            StatusName = t.Status?.Name ?? "",
            DepartmentId = t.DepartmentId,
            DepartmentName = t.Department?.Name ?? "",
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            CreatedBy = t.CreatedBy,
            UpdatedBy = t.UpdatedBy
        };
    }
}
