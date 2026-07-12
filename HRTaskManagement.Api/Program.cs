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
using FluentValidation;
using HRTaskManagement.Application.Validators.Employee;
using HRTaskManagement.Api.Filters;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Application Services (AutoMapper vb.)
builder.Services.AddApplicationServices();

// DbContext kaydı
builder.Services.AddDbContext<WorkSphereDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Infrastructure & Persistence Services
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IPasswordGenerator, PasswordGenerator>();
builder.Services.AddScoped<IJwtService, HRTaskManagement.Infrastructure.Services.JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IAssetAssignmentService, AssetAssignmentService>();
builder.Services.AddScoped<ITaskCommentService, TaskCommentService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeDtoValidator>();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<PasswordChangeEnforcementFilter>();
});

// CORS Konfigürasyonu
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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

    // Admin, Manager veya HR rolüne sahip olanlar
    options.AddPolicy("RequireManagerOrHROrAbove", policy =>
        policy.RequireClaim(ClaimTypes.Role, SystemRoles.Admin, SystemRoles.Manager, SystemRoles.HR));

    // HR rolüne sahip olanlar
    options.AddPolicy("RequireHR", policy =>
        policy.RequireClaim(ClaimTypes.Role, SystemRoles.HR, SystemRoles.Admin));

    // Sisteme giriş yapmış herkes (rol farketmeksizin)
    options.AddPolicy("RequireAuthenticatedUser", policy =>
        policy.RequireAuthenticatedUser());

    // Api/Program.cs
    options.AddPolicy("AdminOrHR", policy =>
        policy.RequireRole(SystemRoles.Admin, SystemRoles.HR));
});

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WorkSphereDbContext>();
    await RoleSeeder.SeedAsync(dbContext);

    // Clean up existing inconsistent soft-deleted user/employee records
    var inconsistentUsers = await dbContext.Users
        .Include(u => u.Employee)
        .Where(u => !u.IsDeleted && u.Employee != null && u.Employee.IsDeleted)
        .ToListAsync();

    foreach (var user in inconsistentUsers)
    {
        user.IsDeleted = true;
        user.IsActive = false;
        if (!user.Username.Contains("__deleted__"))
        {
            user.Username = $"{user.Username}__deleted__{DateTime.UtcNow.Ticks}";
        }
    }

    var inconsistentEmployees = await dbContext.Employees
        .Where(e => e.IsDeleted && !e.Email.Contains("__deleted__"))
        .ToListAsync();

    foreach (var employee in inconsistentEmployees)
    {
        employee.Email = $"{employee.Email}__deleted__{DateTime.UtcNow.Ticks}";
    }

    if (inconsistentUsers.Any() || inconsistentEmployees.Any())
    {
        await dbContext.SaveChangesAsync();
    }

    // Ensure system default positions exist
    var adminPos = await dbContext.Positions.FirstOrDefaultAsync(p => p.Title == "Admin");
    if (adminPos == null)
    {
        adminPos = new Position { Title = "Admin" };
        dbContext.Positions.Add(adminPos);
    }

    var managerPos = await dbContext.Positions.FirstOrDefaultAsync(p => p.Title == "Yönetici");
    if (managerPos == null)
    {
        managerPos = new Position { Title = "Yönetici" };
        dbContext.Positions.Add(managerPos);
    }

    var hrPos = await dbContext.Positions.FirstOrDefaultAsync(p => p.Title == "İnsan Kaynakları");
    if (hrPos == null)
    {
        hrPos = new Position { Title = "İnsan Kaynakları" };
        dbContext.Positions.Add(hrPos);
    }
    await dbContext.SaveChangesAsync();

    // Fix existing employees' positions to match their primary user roles
    var employeesWithWrongPositions = await dbContext.Employees
        .Include(e => e.User)
            .ThenInclude(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
        .Include(e => e.Position)
        .Where(e => !e.IsDeleted && e.User != null)
        .ToListAsync();

    bool needsSave = false;
    foreach (var emp in employeesWithWrongPositions)
    {
        var primaryRole = emp.User.UserRoles.FirstOrDefault()?.Role?.Name;
        if (primaryRole == "Manager" && emp.Position.Title != "Yönetici")
        {
            emp.PositionId = managerPos.Id;
            needsSave = true;
        }
        else if (primaryRole == "Admin" && emp.Position.Title != "Admin")
        {
            emp.PositionId = adminPos.Id;
            needsSave = true;
        }
        else if (primaryRole == "HR" && emp.Position.Title != "İnsan Kaynakları")
        {
            emp.PositionId = hrPos.Id;
            needsSave = true;
        }
    }
    if (needsSave)
    {
        await dbContext.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("AllowFrontend");
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