using Microsoft.EntityFrameworkCore;
using server.Data;

namespace server.Extensions;

public static class EfCoreExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("PG") ?? configuration.GetConnectionString("DefaultConnection"));
        });
        
        return services;
    }
}
