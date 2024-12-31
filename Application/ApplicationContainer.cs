using Application.Profiles;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationContainer
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //builder.Services.AddScoped<IProductService, ProductService>();
            //builder.Services.AddScoped<ICommandHandler<CreateProductCommand>, CreateProductCommandHandler>();
            //builder.Services.AddScoped<IQueryHandler<GetProductsQuery, IEnumerable<Product>>, GetProductsQueryHandler>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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
