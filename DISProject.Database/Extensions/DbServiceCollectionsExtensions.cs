using DISProject.Database.DatabaseContext;
using DISProject.Database.Services;
using DISProject.Database.Services.PeopleServices;
using DISProject.Database.Services.PurchaseServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DISProject.Database.Extensions;

public static class DbServiceCollectionsExtensions
{
    public static IServiceCollection AddDbServices(this IServiceCollection services, string connectionString)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Connection string must be not null or empty", nameof(connectionString));
        }
        
        services.AddDbContext<DISProjectContext>(db =>
            db.UseNpgsql(connectionString,
                b=> b.MigrationsAssembly("DISProject.Api")));

        services.AddScoped<IPeopleService, PeopleService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        

        return services;
    }
    
}