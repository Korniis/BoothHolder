using BoothHolder.IService;
using BoothHolder.Repository;
using BoothHolder.Repository.Impl;
using BoothHolder.Service;
using Microsoft.Extensions.DependencyInjection;

namespace BoothHolder.Extensions.ServiceExtensions
{
    public static class ServiceSetup
    {

        public static void AddServices(this IServiceCollection services)
        {

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBoothService, BoothService>();
            services.AddScoped(typeof(IBaseService<,>), typeof(BaseService<,>));
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IEnterpriseApplicationService, EnterpriseApplicationService>();
        }

        public static void AddRepositorits(this IServiceCollection services)
        {
            services.AddScoped<IBoothRepository, BoothRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEnterpriseApplicationRepository, EnterpriseApplicationRepository>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        }

    }
}
