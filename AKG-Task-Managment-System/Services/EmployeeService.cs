using Microsoft.EntityFrameworkCore;
using AKG_Task_Managment_System.DTOs.Common;
using AKG_Task_Managment_System.DTOs.Employee;
using Task_Managment_Data.Features;
using TMA_Core.Repository;

namespace AKG_Task_Managment_System.Services
{
    public class EmployeeService(IUnitOfWork unitOfWork)
    {
        public async Task<List<EmployeeResponseDto>> GetAllAsync()
        {
            var employees = await unitOfWork.Repository<Employee>().GetQueryable()
                .Include(e => e.Department)
                .ToListAsync();

            return employees.Select(MapToDto).ToList();
        }

        public async Task<EmployeeResponseDto> CreateAsync(CreateEmployeeDto dto, string? userName)
        {
            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                JobTitle = dto.JobTitle,
                DepartmentId = dto.DepartmentId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userName
            };

            await unitOfWork.Repository<Employee>().AddAsync(employee);
            await unitOfWork.SaveChangesAsync();

            var created = await unitOfWork.Repository<Employee>().GetQueryable()
                .Include(e => e.Department)
                .FirstAsync(e => e.Id == employee.Id);

            return MapToDto(created);
        }

        public async Task<EmployeeResponseDto> UpdateAsync(int id, UpdateEmployeeDto dto, string? userName)
        {
            var employee = await unitOfWork.Repository<Employee>().GetQueryable()
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new KeyNotFoundException($"Employee with id {id} not found.");

            if (dto.FirstName != null) employee.FirstName = dto.FirstName;
            if (dto.LastName != null) employee.LastName = dto.LastName;
            if (dto.JobTitle != null) employee.JobTitle = dto.JobTitle;
            if (dto.DepartmentId.HasValue) employee.DepartmentId = dto.DepartmentId.Value;

            employee.UpdatedAt = DateTime.UtcNow;
            employee.UpdatedBy = userName;

            unitOfWork.Repository<Employee>().Update(employee);
            await unitOfWork.SaveChangesAsync();

            var updated = await unitOfWork.Repository<Employee>().GetQueryable()
                .Include(e => e.Department)
                .FirstAsync(e => e.Id == id);

            return MapToDto(updated);
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await unitOfWork.Repository<Employee>().GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Employee with id {id} not found.");

            unitOfWork.Repository<Employee>().Delete(employee);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResponse<EmployeeResponseDto>> GetFilteredAsync(GeneralFilterDto filter)
        {
            IQueryable<Employee> query = unitOfWork.Repository<Employee>().GetQueryable()
                .Include(e => e.Department);

            if (!string.IsNullOrWhiteSpace(filter.GlobalSearch))
            {
                var search = filter.GlobalSearch.ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(search) ||
                    e.LastName.ToLower().Contains(search) ||
                    e.JobTitle.ToLower().Contains(search));
            }

            foreach (var f in filter.Filters)
            {
                query = (f.Field.ToLower(), f.Operator.ToUpper()) switch
                {
                    ("departmentid", "EQUALS") when int.TryParse(f.Value, out var deptId)
                        => query.Where(e => e.DepartmentId == deptId),
                    ("firstname", "CONTAINS")
                        => query.Where(e => e.FirstName.ToLower().Contains(f.Value.ToLower())),
                    ("lastname", "CONTAINS")
                        => query.Where(e => e.LastName.ToLower().Contains(f.Value.ToLower())),
                    _ => query
                };
            }

            var totalElements = await query.CountAsync();

            query = filter.SortBy.ToLower() switch
            {
                "firstname" => filter.SortDirection.ToLower() == "asc"
                    ? query.OrderBy(e => e.FirstName) : query.OrderByDescending(e => e.FirstName),
                "lastname" => filter.SortDirection.ToLower() == "asc"
                    ? query.OrderBy(e => e.LastName) : query.OrderByDescending(e => e.LastName),
                "department" => filter.SortDirection.ToLower() == "asc"
                    ? query.OrderBy(e => e.Department!.Name) : query.OrderByDescending(e => e.Department!.Name),
                _ => filter.SortDirection.ToLower() == "asc"
                    ? query.OrderBy(e => e.Id) : query.OrderByDescending(e => e.Id)
            };

            var items = await query.Skip(filter.Page * filter.Size).Take(filter.Size).ToListAsync();

            return new PagedResponse<EmployeeResponseDto>
            {
                Content = items.Select(MapToDto).ToList(),
                TotalElements = totalElements,
                TotalPages = (int)Math.Ceiling(totalElements / (double)filter.Size),
                Size = filter.Size,
                Page = filter.Page
            };
        }

        private static EmployeeResponseDto MapToDto(Employee e) => new()
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            JobTitle = e.JobTitle,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department?.Name ?? ""
        };
    }
}
