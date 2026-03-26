using Microsoft.EntityFrameworkCore;
using AKG_Task_Managment_System.DTOs.Auth;
using AKG_Task_Managment_System.DTOs.Common;
using Task_Managment_Data.Auth;
using TMA_Core.Repository;

namespace AKG_Task_Managment_System.Services
{
    public class UserService(IUnitOfWork unitOfWork)
    {
        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            var users = await unitOfWork.Repository<User>().GetQueryable()
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.Employee)
                .ToListAsync();

            return users.Select(MapToDto).ToList();
        }

        public async Task<UserResponseDto> CreateAsync(CreateUserDto dto, string? userName)
        {
            var user = new User
            {
                UserName = dto.UserName,
                PasswordHash = AuthService.HashPassword(dto.Password),
                Email = dto.Email,
                EmployeeId = dto.EmployeeId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userName
            };

            user.UserRoles.Add(new UserRole { RoleId = dto.RoleId });
            await unitOfWork.Repository<User>().AddAsync(user);
            await unitOfWork.SaveChangesAsync();

            var created = await unitOfWork.Repository<User>().GetQueryable()
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.Employee)
                .FirstAsync(u => u.Id == user.Id);

            return MapToDto(created);
        }

        public async Task<UserResponseDto> UpdateAsync(int id, UpdateUserDto dto, string? userName)
        {
            var user = await unitOfWork.Repository<User>().GetQueryable()
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new KeyNotFoundException($"User with id {id} not found.");

            if (dto.Email != null) user.Email = dto.Email;
            if (dto.Password != null) user.PasswordHash = AuthService.HashPassword(dto.Password);

            if (dto.RoleId.HasValue)
            {
                user.UserRoles.Clear();
                user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = dto.RoleId.Value });
            }

            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = userName;

            unitOfWork.Repository<User>().Update(user);
            await unitOfWork.SaveChangesAsync();

            var updated = await unitOfWork.Repository<User>().GetQueryable()
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.Employee)
                .FirstAsync(u => u.Id == id);

            return MapToDto(updated);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await unitOfWork.Repository<User>().GetQueryable()
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new KeyNotFoundException($"User with id {id} not found.");

            user.UserRoles.Clear();
            unitOfWork.Repository<User>().Delete(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResponse<UserResponseDto>> GetFilteredAsync(GeneralFilterDto filter)
        {
            IQueryable<User> query = unitOfWork.Repository<User>().GetQueryable()
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.Employee);

            if (!string.IsNullOrWhiteSpace(filter.GlobalSearch))
            {
                var search = filter.GlobalSearch.ToLower();
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(search) ||
                    (u.Email != null && u.Email.ToLower().Contains(search)));
            }

            foreach (var f in filter.Filters)
            {
                query = (f.Field.ToLower(), f.Operator.ToUpper()) switch
                {
                    ("role", "EQUALS") => query.Where(u =>
                        u.UserRoles.Any(ur => ur.Role.Name.ToLower() == f.Value.ToLower())),
                    ("username", "CONTAINS") => query.Where(u =>
                        u.UserName.ToLower().Contains(f.Value.ToLower())),
                    _ => query
                };
            }

            var totalElements = await query.CountAsync();

            query = filter.SortBy.ToLower() switch
            {
                "username" => filter.SortDirection.ToLower() == "asc"
                    ? query.OrderBy(u => u.UserName) : query.OrderByDescending(u => u.UserName),
                "email" => filter.SortDirection.ToLower() == "asc"
                    ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
                _ => filter.SortDirection.ToLower() == "asc"
                    ? query.OrderBy(u => u.Id) : query.OrderByDescending(u => u.Id)
            };

            var items = await query.Skip(filter.Page * filter.Size).Take(filter.Size).ToListAsync();

            return new PagedResponse<UserResponseDto>
            {
                Content = items.Select(MapToDto).ToList(),
                TotalElements = totalElements,
                TotalPages = (int)Math.Ceiling(totalElements / (double)filter.Size),
                Size = filter.Size,
                Page = filter.Page
            };
        }

        private static UserResponseDto MapToDto(User u) => new()
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            EmployeeId = u.EmployeeId,
            EmployeeName = u.Employee != null
                ? $"{u.Employee.FirstName} {u.Employee.LastName}" : "",
            Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
    }
}
