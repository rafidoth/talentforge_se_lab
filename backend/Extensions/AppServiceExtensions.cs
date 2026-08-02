using server.Services.UserServices;

namespace server.Extensions;

public static class AppServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserPreferenceService, UserPreferenceService>();
        services.AddScoped<server.Services.ProfileServices.IProfileService, server.Services.ProfileServices.ProfileService>();
        services.AddScoped<server.Services.AttributeLibraryServices.IAttributeService, server.Services.AttributeLibraryServices.AttributeService>();
        return services;
    }
}
