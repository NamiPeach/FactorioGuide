using Factorio.Application.Abstractions;
using Factorio.Infrastructure.Authentication;
using Factorio.Infrastructure.Data;
using Factorio.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Factorio.Infrastructure;

/// <summary>Registers everything from the Infrastructure layer in the DI container.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

        // Port -> adapter: the rest of the app asks for an interface and gets the implementation
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IActionRepository, ActionRepository>();

        return services;
    }
}
