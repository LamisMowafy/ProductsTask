using Application.Profiles;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Resource;

namespace Application
{
    public static class ApplicationContainer
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services) // Add IConfiguration parameter
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IResourceHelper, ResourceHelper>();
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
            services.AddMediatR(config =>
            {
                // Pass the assembly where the MediatR handlers are located
                config.RegisterServicesFromAssembly(typeof(ApplicationContainer).Assembly);
            });
            return services;
        }
    }
}
