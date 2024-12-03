
using eCommerce.Core.RepositroyContracts;
using eCommerce.Infrastructure.DbContext;
using eCommerce.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            //TODO: Add services to the IoC container
            services.AddTransient<IUsersRepository, UsersRepository>();
            services.AddTransient<DapperDbContext>();
            return services;
        }
    }
}
