using System.Text;
using System.Text.Json.Serialization;

using HirePath.Mappings;

using HirePathAI.API.Configuration;
using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Implementations;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Auth;
using HirePathAI.API.Services.CompanyOnboarding;
using HirePathAI.API.Services.Implementations;
using HirePathAI.API.Services.Interfaces;
using HirePathAI.API.Services.PlatformAdmin;

using HirePathAI.Repositories;
using HirePathAI.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ====================================================
// DATABASE
// ====================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString =
        builder.Configuration.GetConnectionString(
            "DefaultConnection");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "DefaultConnection is missing from appsettings.json.");
    }

    options.UseSqlServer(connectionString);
});

// ====================================================
// IDENTITY
// ====================================================

builder.Services
    .AddIdentity<User, IdentityRole<int>>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;

        options.User.RequireUniqueEmail = true;

        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(5);

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ====================================================
// EMAIL SETTINGS AND OTP
// ====================================================

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOtpService, OtpService>();

// ====================================================
// CLOUDINARY / CLOUD STORAGE
// ====================================================

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddScoped<
    ICloudStorageService,
    CloudinaryStorageService>();

// ====================================================
// JWT SERVICE
// ====================================================

builder.Services.AddScoped<
    IJwtTokenService,
    JwtTokenService>();

// ====================================================
// JWT AUTHENTICATION
// ====================================================

var jwtSettings =
    builder.Configuration.GetSection("Jwt");

var jwtKey = jwtSettings["Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT Key is missing from appsettings.json.");
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    jwtSettings["Issuer"],

                ValidAudience =
                    jwtSettings["Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                ClockSkew = TimeSpan.Zero
            };
    });

// ====================================================
// AUTHORIZATION
// ====================================================

builder.Services.AddAuthorization();

// ====================================================
// REPOSITORIES
// ====================================================

builder.Services.AddScoped<
    IUserRepository,
    UserRepository>();

builder.Services.AddScoped<
    IJobRepository,
    JobRepository>();

builder.Services.AddScoped<
    ICandidateRepository,
    CandidateRepository>();

builder.Services.AddScoped<
    IApplicationRepository,
    ApplicationRepository>();

builder.Services.AddScoped<
    IInterviewRepository,
    InterviewRepository>();

builder.Services.AddScoped<
    IInterviewFeedbackRepository,
    InterviewFeedbackRepository>();

builder.Services.AddScoped<
    IEvaluationRepository,
    EvaluationRepository>();

builder.Services.AddScoped<
    IApplicationStatusHistoryRepository,
    ApplicationStatusHistoryRepository>();

builder.Services.AddScoped<
    ICompanyRepository,
    CompanyRepository>();

builder.Services.AddScoped(
    typeof(IGenericRepository<>),
    typeof(GenericRepository<>));

// ====================================================
// RECRUITER MODULE
// ====================================================

builder.Services.AddScoped<
    IRecruiterRepository,
    RecruiterRepository>();

builder.Services.AddScoped<
    IRecruiterService,
    RecruiterService>();

// ====================================================
// APPLICATION SERVICES
// ====================================================

builder.Services.AddScoped<
    IAuthService,
    AuthService>();

builder.Services.AddScoped<
    IJobService,
    JobService>();

builder.Services.AddScoped<
    ICandidateService,
    CandidateService>();

builder.Services.AddScoped<
    IApplicationService,
    ApplicationService>();

builder.Services.AddScoped<
    IInterviewService,
    InterviewService>();

builder.Services.AddScoped<
    IInterviewFeedbackService,
    InterviewFeedbackService>();

builder.Services.AddScoped<
    IEvaluationService,
    EvaluationService>();

builder.Services.AddScoped<
    ICompanyService,
    CompanyService>();

builder.Services.AddScoped<
    IAIService,
    AIService>();

// ====================================================
// PLATFORM ADMIN / SUPER ADMIN
// ====================================================

builder.Services.AddScoped<
    IPlatformAdminService,
    PlatformAdminService>();

// ====================================================
// COMPANY ONBOARDING / COMPANY ADMIN
// ====================================================

builder.Services.AddScoped<
    ICompanyOnboardingService,
    CompanyOnboardingService>();

// ====================================================
// AUTOMAPPER
// ====================================================

builder.Services.AddAutoMapper(
    typeof(AutoMapperProfile));

// ====================================================
// CORS
// ====================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var configuredOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? Array.Empty<string>();

        var allowedOrigins = new[]
        {
            "http://localhost:5173",
            "http://127.0.0.1:5173",
            "https://localhost:5173",
            "https://127.0.0.1:5173"
        }
        .Concat(configuredOrigins)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ====================================================
// CONTROLLERS
// ====================================================

builder.Services
    .AddControllers()
    .AddNewtonsoftJson()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

// ====================================================
// SWAGGER
// ====================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "HirePath AI API",
            Version = "v1",
            Description =
                "HirePath AI recruitment and company onboarding API"
        });

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Enter your JWT token. Example: Bearer {token}"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type =
                                ReferenceType.SecurityScheme,

                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});

var app = builder.Build();

// ====================================================
// SEED ROLES AND PLATFORM ADMIN
// ====================================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        await SeedData.SeedRolesAndAdminAsync(
            services);
    }
    catch (Exception exception)
    {
        var logger =
            services.GetRequiredService<
                ILogger<Program>>();

        logger.LogError(
            exception,
            "An error occurred while seeding roles and admin.");
    }
}

// ====================================================
// MIDDLEWARE PIPELINE
// ====================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// In local development the React app calls the HTTP profile on port 5139.
// Redirecting an OPTIONS preflight request to HTTPS causes the browser to block it.
// Keep HTTPS redirection for production, but do not redirect local development traffic.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// CORS must execute before authentication/authorization.
app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// ====================================================
// START PAGE
// ====================================================

app.MapGet(
    "/",
    async context =>
    {
        context.Response.ContentType =
            "text/html";

        await context.Response.WriteAsync(
            """
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">

                <meta
                    name="viewport"
                    content="width=device-width, initial-scale=1.0">

                <title>HirePath AI</title>

                <style>
                    * {
                        box-sizing: border-box;
                    }

                    body {
                        margin: 0;
                        min-height: 100vh;
                        display: flex;
                        align-items: center;
                        justify-content: center;
                        padding: 24px;
                        font-family: Arial, sans-serif;
                        background:
                            linear-gradient(
                                135deg,
                                #07111f,
                                #0f2742
                            );
                        color: white;
                    }

                    .card {
                        width: 100%;
                        max-width: 460px;
                        padding: 40px;
                        border-radius: 20px;
                        text-align: center;
                        background:
                            rgba(
                                255,
                                255,
                                255,
                                0.10
                            );
                        box-shadow:
                            0 20px 50px
                            rgba(
                                0,
                                0,
                                0,
                                0.30
                            );
                        backdrop-filter: blur(12px);
                        border:
                            1px solid
                            rgba(
                                255,
                                255,
                                255,
                                0.12
                            );
                    }

                    h1 {
                        margin-top: 0;
                        margin-bottom: 10px;
                        color: #66b2ff;
                        font-size: 38px;
                    }

                    p {
                        color: #d7e7ff;
                        margin-bottom: 30px;
                        line-height: 1.6;
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
                        transition:
                            transform 0.2s,
                            background 0.2s;
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
                        transform: translateY(-2px);
                    }

                    .note {
                        margin-top: 25px;
                        font-size: 13px;
                        color: #aac7e8;
                        line-height: 1.6;
                    }
                </style>
            </head>

            <body>
                <div class="card">
                    <h1>HirePath AI</h1>

                    <p>
                        Recruitment, company onboarding and
                        AI-assisted candidate management platform.
                    </p>

                    <a
                        class="btn"
                        href="/swagger">

                        Open Swagger API
                    </a>

                    <a
                        class="btn secondary"
                        href="http://localhost:5173">

                        Open Frontend UI
                    </a>

                    <div class="note">
                        Backend API is running successfully.
                        <br>
                        Frontend URL:
                        http://localhost:5173
                    </div>
                </div>
            </body>
            </html>
            """);
    });

app.Run();