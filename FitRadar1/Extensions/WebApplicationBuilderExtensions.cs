using FitRadar.Data;
using FitRadar.Repositories.Interfaces;
using FitRadar.Repositories;
using FitRadar.Services;
using FitRadar.Services.Interfaces;
using FitRadar.Shared.Models;
using FitRadar.Shared.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace FitRadar.Extensions
{
    /// <summary>
    /// Extension methods for WebApplicationBuilder to keep Program.cs clean
    /// </summary>
    public static class WebApplicationBuilderExtensions
    {
        public static void AddFitRadarDbContext(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection not configured");

            builder.Services.AddDbContext<FitRadarDbContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(3),
                        errorNumbersToAdd: null
                    );
                })
            );
        }

        public static void AddFitRadarIdentity(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddIdentity<User, IdentityRole<Guid>>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = false;

                    options.SignIn.RequireConfirmedEmail = false;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<FitRadarDbContext>()
                .AddDefaultTokenProviders();
        }

        public static void AddFitRadarAuthentication(this WebApplicationBuilder builder)
        {
            var jwtSettings = new JwtSettings();
            builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);

            jwtSettings.SecretKey = builder.Configuration["JwtSettings:SecretKey"]
                ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                ?? throw new InvalidOperationException(
                    "JWT Secret Key not configured. Set 'JwtSettings:SecretKey' in User Secrets or 'JWT_SECRET_KEY' environment variable.");

            builder.Services.AddSingleton(jwtSettings);

            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        public static void AddFitRadarRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IFacilityRepository, EfFacilityRepository>();
            builder.Services.AddScoped<IPackageRepository, EfPackageRepository>();
            builder.Services.AddScoped<IProviderRepository, EfProviderRepository>();
            builder.Services.AddScoped<IUserRepository, EfUserRepository>();
        }

        public static void AddFitRadarServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<TokenService>();
            builder.Services.AddScoped<IFacilityService, FacilityService>();
            builder.Services.AddScoped<IPackageService, PackageService>();
            builder.Services.AddScoped<IProviderService, ProviderService>();
            builder.Services.AddScoped<IUserService, UserService>();
        }

        public static void AddFitRadarSwagger(this WebApplicationBuilder builder)
        {
            if (builder.Environment.IsProduction())
                return;

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FitRadar API",
                    Version = "v1",
                    Description = "API for FitRadar - fitness facility comparison and subscription management"
                });

                // Add JWT Authorization to Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        []
                    }
                });
            });
        }

        public static void AddFitRadarCors(this WebApplicationBuilder builder)
        {
            var frontendUrl = builder.Configuration["Cors:FrontendUrl"]
                ?? Environment.GetEnvironmentVariable("FRONTEND_URL")
                ?? "http://localhost:3000"; // Default for dev

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins(frontendUrl)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }
    }
}