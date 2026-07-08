using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Implementations;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using HirePathAI.API.Services.Interfaces;
using HirePathAI.API.Services.Implementations;
using HirePathAI.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// Database
// ----------------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ----------------------------------------------------
// Identity
// ----------------------------------------------------
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ----------------------------------------------------
// Email + OTP
// ----------------------------------------------------
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOtpService, OtpService>();

//
// cloud storage service
//
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddScoped<ICloudStorageService, CloudinaryStorageService>();

// ----------------------------------------------------
// JWT Service
// ----------------------------------------------------
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// ----------------------------------------------------
// JWT Authentication
// ----------------------------------------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"];

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
        ClockSkew = TimeSpan.Zero
    };
});

// ----------------------------------------------------
// Repositories
// ----------------------------------------------------
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();

// ----------------------------------------------------
// Services
// ----------------------------------------------------
builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();
builder.Services.AddScoped<IAIService, AIService>();

// ----------------------------------------------------
// CORS for React frontend
// ----------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ----------------------------------------------------
// Controllers + Swagger
// ----------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HirePath AI API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ----------------------------------------------------
// Seed Roles + Admin
// ----------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.SeedRolesAndAdminAsync(services);
}

// ----------------------------------------------------
// Middleware pipeline
// ----------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ----------------------------------------------------
// Start Page: Choose Swagger or Frontend
// ----------------------------------------------------
app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html";

    await context.Response.WriteAsync("""
    <!DOCTYPE html>
    <html>
    <head>
        <title>HirePath AI</title>
        <style>
            body {
                margin: 0;
                min-height: 100vh;
                display: flex;
                align-items: center;
                justify-content: center;
                font-family: Arial, sans-serif;
                background: linear-gradient(135deg, #07111f, #0f2742);
                color: white;
            }

            .card {
                width: 460px;
                background: rgba(255, 255, 255, 0.1);
                padding: 40px;
                border-radius: 20px;
                text-align: center;
                box-shadow: 0 20px 50px rgba(0,0,0,0.3);
                backdrop-filter: blur(12px);
            }

            h1 {
                margin-bottom: 10px;
                color: #66b2ff;
            }

            p {
                color: #d7e7ff;
                margin-bottom: 30px;
            }

            .btn {
                display: block;
                margin: 14px 0;
                padding: 15px;
                border-radius: 12px;
                text-decoration: none;
                font-weight: bold;
                color: white;
                background: #0d6efd;
                transition: 0.2s;
            }

            .btn:hover {
                background: #0b5ed7;
                transform: translateY(-2px);
            }

            .secondary {
                background: #198754;
            }

            .secondary:hover {
                background: #157347;
            }

            .note {
                margin-top: 25px;
                font-size: 13px;
                color: #aac7e8;
            }
        </style>
    </head>
    <body>
        <div class="card">
            <h1>HirePath AI</h1>
            <p>Choose how you want to continue</p>

            <a class="btn" href="/swagger">Open Swagger API</a>
            <a class="btn secondary" href="http://localhost:5173">Open Frontend UI</a>

            <div class="note">
                Backend API must be running here. Frontend must be running on http://localhost:5173
            </div>
        </div>
    </body>
    </html>
    """);
});

app.Run();