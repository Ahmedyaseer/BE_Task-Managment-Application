using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AKG_Task_Managment_System.DTOs.Auth;
using Task_Managment_Data.Auth;
using TMA_Core.Repository;

namespace AKG_Task_Managment_System.Services
{
    public class AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await unitOfWork.Repository<User>().GetQueryable()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == dto.UserName)
                ?? throw new Exception("Invalid username or password");

            var hash = HashPassword(dto.Password);
            if (user.PasswordHash != hash)
                throw new Exception("Invalid username or password");

            var role = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "Employee";
            var roleId = user.UserRoles.FirstOrDefault()?.RoleId ?? 0;

            // Load permissions separately to avoid deep include issues
            var permissions = await unitOfWork.Repository<RolePermission>().GetQueryable()
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToListAsync();

            var token = GenerateJwtToken(user, role, permissions);
            var expiresIn = int.Parse(configuration["Jwt:ExpirationInMinutes"] ?? "60");

            return new LoginResponseDto
            {
                Token = token,
                UserName = user.UserName,
                Email = user.Email,
                Role = role,
                Permissions = permissions,
                ExpiresInMinutes = expiresIn
            };
        }

        private string GenerateJwtToken(User user, string role, List<string> permissions)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresIn = int.Parse(configuration["Jwt:ExpirationInMinutes"] ?? "60");

            var claims = new List<Claim>
            {
                new("userId", user.Id.ToString()),
                new("userName", user.UserName),
                new("email", user.Email ?? ""),
                new("role", role)
            };

            foreach (var permission in permissions)
                claims.Add(new Claim("permission", permission));

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresIn),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string HashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
