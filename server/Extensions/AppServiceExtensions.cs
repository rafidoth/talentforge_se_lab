using Microsoft.AspNetCore.Identity;
using server.Entities;
using server.Services.AttributeLibraryServices;
using server.Services.ProfileServices;
using server.Services.ProjectsServices;
using server.Services.TagsServices;
using server.Services.UserServices;
using server.Services.CloudinaryServices;
using server.Utils;

namespace server.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAttributeService, AttributeService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserConfirmation<ApplicationUser>, UserConfirmation>();
            services.AddScoped<IProjectsService, ProjectsService>();
            services.AddScoped<ITagsService, TagsService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IUserPreferenceService, UserPreferenceService>();
            return services;
        }
    }
}