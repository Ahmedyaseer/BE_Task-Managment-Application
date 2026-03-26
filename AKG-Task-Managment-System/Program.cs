using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Task_Managment_Data.Core;
using AKG_Task_Managment_System.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataServices(builder.Configuration);
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<DepartmentService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            RoleClaimType = "role",
            NameClaimType = "userName"
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CanCreateTask", p => p.RequireClaim("permission", "Task.Create"))
    .AddPolicy("CanReadTask", p => p.RequireClaim("permission", "Task.Read"))
    .AddPolicy("CanUpdateTask", p => p.RequireClaim("permission", "Task.Update"))
    .AddPolicy("CanDeleteTask", p => p.RequireClaim("permission", "Task.Delete"))
    .AddPolicy("CanCompleteTask", p => p.RequireClaim("permission", "Task.Complete"))
    .AddPolicy("CanManageUsers", p => p.RequireClaim("permission", "User.Manage"));

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Initialize(context);
    app.MapOpenApi();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
