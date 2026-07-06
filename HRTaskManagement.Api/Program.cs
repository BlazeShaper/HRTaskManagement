using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using HRTaskManagement.Application;
using HRTaskManagement.Persistence.Context;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Infrastructure.Services;
using HRTaskManagement.Persistence.Services;
using HRTaskManagement.Persistence.Seed;
using HRTaskManagement.Shared.Constants;
using HRTaskManagement.Api.Middleware;
using HRTaskManagement.Api.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Application Services (AutoMapper vb.)
builder.Services.AddApplicationServices();

// DbContext kaydı
builder.Services.AddDbContext<WorkSphereDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtService, HRTaskManagement.Infrastructure.Services.JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
// ============================================
// JWT Authentication Konfigürasyonu
// ============================================

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
        ClockSkew = TimeSpan.Zero   // Token süresi dolduğu an geçersiz sayılsın, ekstra tolerans verme
    };
});

builder.Services.AddAuthorization(options =>
{
    // Sadece Admin rolüne sahip olanlar
    options.AddPolicy("RequireAdmin", policy =>
        policy.RequireClaim(ClaimTypes.Role, SystemRoles.Admin));

    // Admin veya Manager rolüne sahip olanlar
    options.AddPolicy("RequireManagerOrAbove", policy =>
        policy.RequireClaim(ClaimTypes.Role, SystemRoles.Admin, SystemRoles.Manager));

    // HR rolüne sahip olanlar
    options.AddPolicy("RequireHR", policy =>
        policy.RequireClaim(ClaimTypes.Role, SystemRoles.HR, SystemRoles.Admin));

    // Sisteme giriş yapmış herkes (rol farketmeksizin)
    options.AddPolicy("RequireAuthenticatedUser", policy =>
        policy.RequireAuthenticatedUser());
});

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WorkSphereDbContext>();
    await RoleSeeder.SeedAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

// ============================================
// Authentication & Authorization Middleware
// ============================================
app.UseAuthentication();   // "Bu kullanıcı kim?" — token'ı doğrular
app.UseAuthorization();    // "Bu kullanıcı bunu yapabilir mi?" — yetki kontrolü

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}