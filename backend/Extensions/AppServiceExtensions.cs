using server.Services.UserServices;

namespace server.Extensions;

public static class AppServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserPreferenceService, UserPreferenceService>();
        return services;
    }
}
