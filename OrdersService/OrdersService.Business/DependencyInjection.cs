
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersService.Business.Mappers;
using OrdersService.Business.ServiceContracts;
using OrdersService.Business.Validators;
using os = OrdersService.Business.Services;

namespace OrdersService.Business
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessDependencies(
            this IServiceCollection services,
            IConfiguration configuration
            )
        {
            services.AddValidatorsFromAssemblyContaining<OrderAddRequestValidator>();
            services.AddAutoMapper(typeof(OrderAddRequestToOrderMappingProfile).Assembly);
            services.AddScoped<IOrdersService, os.OrdersService>();
            return services;
        }

    }
}
