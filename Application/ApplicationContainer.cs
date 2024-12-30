using Application.Profiles;
using Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationContainer
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
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
