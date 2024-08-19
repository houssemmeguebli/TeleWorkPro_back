using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.RateLimiting;
using TTProject.Application.Services;
using TTProject.Core.Entities;
using TTProject.Core.Interfaces;
using TTProject.Infrastructure;
using TTProject.Infrastructure.Data;
using TTProject.Infrastructure.Repositories;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProjectManagerOnly", policy =>
    {
        policy.RequireRole("ProjectManager");
    });
    options.AddPolicy("EmployeeOnly", policy =>
    {
        policy.RequireRole("Employee");
    });
});


// Configure DbContext for Entity Framework Core
builder.Services.AddDbContext<TTProjectContextOld>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity services with custom User class
builder.Services.AddIdentity<User, IdentityRole<long>>()
    .AddEntityFrameworkStores<TTProjectContextOld>()
    .AddDefaultTokenProviders();

// Configure application-specific services
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProjectManagerService, ProjectManagerService>();
builder.Services.AddScoped<IProjectManagerRepository, ProjectManagerRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IEmplyeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Load EmailSettings from configuration
var emailSettings = builder.Configuration.GetSection("EmailSettings");

// Add EmailService to the service collection
builder.Services.AddSingleton<IEmailService>(new EmailService(
    emailSettings["Username"],
    emailSettings["Password"]
));

// Add UserManager and SignInManager services
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<SignInManager<User>>();

// Configure JWT authentication
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials(); // Allow credentials to be included in requests
        });
});

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireProjectManagerRole", policy =>
        policy.RequireRole("ProjectManager"));

    options.AddPolicy("RequireEmployeeRole", policy =>
        policy.RequireRole("Employee"));
});

// Configure Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("fixed", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: partiction => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromSeconds(10)
            }));
});

var app = builder.Build();

// Initialize the database with roles
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<long>>>();
    await DbInitializer.InitializeAsync(userManager, roleManager);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Apply CORS policy
app.UseCors("AllowSpecificOrigin");

// Add rate limiting middleware
app.UseRateLimiter();

app.UseAuthentication(); // Ensure the authentication middleware is added
app.UseAuthorization();  // Ensure the authorization middleware is added

app.MapControllers();
app.Run();
