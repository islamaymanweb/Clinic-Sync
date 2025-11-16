using Core.Entities;
using Core.interfaces;
using Core.Services;
using infrastructure.Data;
using infrastructure.Repositories;
using infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static infrastructure.Services.EmailService;

namespace infrastructure
{
    public static class infrastructureRegisteration
    {
        public static IServiceCollection infrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            // Identity
            services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;

                // User settings
                options.User.RequireUniqueEmail = true;
   
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // JWT Configuration for Token Generation
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "ClinicSync.Auth";
                options.Cookie.HttpOnly = true;
                // ✅ في Development: استخدام SameSite.None للسماح بالـ cookies عبر origins مختلفة
                // في Production: يمكن استخدام SameSiteMode.Strict
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // ✅ يعمل مع http و https
                options.Cookie.SameSite = SameSiteMode.Lax; // ✅ أفضل من Strict للـ API calls
                options.Cookie.MaxAge = TimeSpan.FromDays(7); // Remember me duration
                options.LoginPath = "/api/auth/login";
                options.AccessDeniedPath = "/api/auth/access-denied";
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.SlidingExpiration = true;

                // Events for additional control
                options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = async context =>
                    {
                        // Custom validation logic if needed
                        await Task.CompletedTask;
                    },
                    OnRedirectToLogin = context =>
                    {
                        // ✅ للـ API calls، نرجع 401 بدلاً من redirect
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        }
                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    }
                };
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"])),
                    ClockSkew = TimeSpan.Zero
                };
                
                // ✅ إضافة دعم لقراءة JWT token من الـ cookie
                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // ✅ محاولة قراءة الـ token من Authorization header أولاً
                        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                        
                        // ✅ إذا لم يكن موجوداً في header، نقرأه من cookie
                        if (string.IsNullOrEmpty(token))
                        {
                            token = context.Request.Cookies["ClinicSync.Auth"];
                        }
                        
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }
                        
                        return Task.CompletedTask;
                    }
                };
            });

            // Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("DoctorOnly", policy => policy.RequireRole("Doctor"));
                options.AddPolicy("PatientOnly", policy => policy.RequireRole("Patient"));
            });

            // Email Configuration
            services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
            services.AddScoped<IEmailService, EmailService>();

            // Application Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
          
             services.AddScoped<IDoctorService, DoctorService>();
            // في Program.cs
            // Add Repositories and Services
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAppointmentService, AppointmentService>();


            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowClinicSync", policy =>
                {
                    policy.WithOrigins(
                          "http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                           .AllowCredentials()  
                          .WithExposedHeaders("X-Pagination", "X-Total-Count");
                    //    .AllowCredentials().AllowCredentials() // Important for cookies
                    //.WithExposedHeaders("Set-Cookie"); // Expose Set-Cookie header
                });
            });

            return services;
        
        }
    }
}
