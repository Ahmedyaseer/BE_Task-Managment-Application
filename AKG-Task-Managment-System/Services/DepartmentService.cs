using Microsoft.EntityFrameworkCore;
using AKG_Task_Managment_System.DTOs.Common;
using AKG_Task_Managment_System.DTOs.Employee;
using Task_Managment_Data.Features;
using TMA_Core.Repository;

namespace AKG_Task_Managment_System.Services
{
    public class DepartmentService(IUnitOfWork unitOfWork)
    {
        public async Task<List<DepartmentDto>> GetAllAsync()
        {
            var departments = await unitOfWork.Repository<Department>().GetAllAsync();
            return departments.Select(d => new DepartmentDto { Id = d.Id, Name = d.Name }).ToList();
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, string? userName)
        {
            var dept = new Department
            {
                Name = dto.Name,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userName
            };

            await unitOfWork.Repository<Department>().AddAsync(dept);
            await unitOfWork.SaveChangesAsync();

            return new DepartmentDto { Id = dept.Id, Name = dept.Name };
        }

        public async Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto dto, string? userName)
        {
            var dept = await unitOfWork.Repository<Department>().GetByIdAsync(id)
                ?? throw new Exception("Department not found");

            if (dto.Name is not null) dept.Name = dto.Name;
            dept.UpdatedAt = DateTime.UtcNow;
            dept.UpdatedBy = userName;

            unitOfWork.Repository<Department>().Update(dept);
            await unitOfWork.SaveChangesAsync();

            return new DepartmentDto { Id = dept.Id, Name = dept.Name };
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var dept = await unitOfWork.Repository<Department>().GetByIdAsync(id)
                ?? throw new Exception("Department not found");

            unitOfWork.Repository<Department>().Delete(dept);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResponse<DepartmentDto>> GetFilteredAsync(GeneralFilterDto filter)
        {
            var query = unitOfWork.Repository<Department>().GetQueryable();

            if (!string.IsNullOrWhiteSpace(filter.GlobalSearch))
            {
                var search = filter.GlobalSearch.ToLower();
                query = query.Where(d => d.Name.ToLower().Contains(search));
            }

            foreach (var f in filter.Filters)
            {
                query = (f.Field.ToLower(), f.Operator.ToUpper()) switch
                {
                    ("name", "CONTAINS") => query.Where(d => d.Name.ToLower().Contains(f.Value.ToLower())),
                    _ => query
                };
            }

            var totalElements = await query.CountAsync();

            query = filter.SortBy.ToLower() switch
            {
                "name" => filter.SortDirection.ToLower() == "asc"
                    ? query.OrderBy(d => d.Name) : query.OrderByDescending(d => d.Name),
                _ => filter.SortDirection.ToLower() == "asc"
                    ? query.OrderBy(d => d.Id) : query.OrderByDescending(d => d.Id)
            };

            var items = await query.Skip(filter.Page * filter.Size).Take(filter.Size).ToListAsync();

            return new PagedResponse<DepartmentDto>
            {
                Content = items.Select(d => new DepartmentDto { Id = d.Id, Name = d.Name }).ToList(),
                TotalElements = totalElements,
                TotalPages = (int)Math.Ceiling(totalElements / (double)filter.Size),
                Size = filter.Size,
                Page = filter.Page
            };
        }
    }
}
