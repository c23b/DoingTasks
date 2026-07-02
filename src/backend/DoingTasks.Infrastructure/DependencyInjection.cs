using DoingTasks.Application.Abstractions.Authentication;
using DoingTasks.Application.Abstractions.Data;
using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Domain.Tasks;
using DoingTasks.Domain.Users;
using DoingTasks.Domain.Workspaces;
using DoingTasks.Infrastructure.Authentication.Google;
using DoingTasks.Infrastructure.Authentication.Identity;
using DoingTasks.Infrastructure.Authentication.Microsoft;
using DoingTasks.Infrastructure.Authentication.Token;
using DoingTasks.Infrastructure.DomainEvents;
using DoingTasks.Infrastructure.ExternalServices.Google;
using DoingTasks.Infrastructure.ExternalServices.Microsoft;
using DoingTasks.Infrastructure.Messaging;
using DoingTasks.Infrastructure.Persistence;
using DoingTasks.Infrastructure.Persistence.Repositories;
using DoingTasks.Infrastructure.Time;
using DoingTasks.SharedKernel.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Refit;
using System.Text;

namespace DoingTasks.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        // Unit of Work
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
        services.AddScoped<IWorkTaskRepository, WorkTaskRepository>();

        // Domain Events
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Messaging
        services.AddScoped<IMediator, Mediator>();

        // Time
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Identity
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityProvider, IdentityProvider>();

        // JWT
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<ITokenProvider, TokenProvider>();

        // Authentication
        services.AddScoped<IGoogleAuthProvider, GoogleAuthProvider>();
        services.AddScoped<IMicrosoftAuthProvider, MicrosoftAuthProvider>();

        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
        services.AddAuthentication(options =>
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
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
            };
        });

        // External Services
        services.AddRefitClient<IGoogleAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://oauth2.googleapis.com"));

        services.AddRefitClient<IMicrosoftAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://graph.microsoft.com"));

        return services;
    }
}