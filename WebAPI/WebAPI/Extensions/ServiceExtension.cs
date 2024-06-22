using System.Net.NetworkInformation;
using WebAPI.Interfaces;
using WebAPI.Repositories;

namespace WebAPI.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddUserServices(this IServiceCollection services)
        {
            services.AddScoped<IUser, UserRepository>();
            //Add more scoped.

            return services;
        }
    }
}
