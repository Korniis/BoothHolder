
using BoothHolder.Common;
using BoothHolder.Common.Configration;
using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using BoothHolder.Model.VO;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BoothHolder.Extensions.ServiceExtensions
{
    public static class ConfigureSetup
    {
        public static void AddConfiguration(this IServiceCollection services, WebApplicationBuilder builder)
        {

            services.AddSingleton(new AppSettings(builder.Configuration));
            //builder.Services.AddSingleton(new JwtHelper());

            services.AddSingleton(new MD5Helper());
            services.AddSingleton(new SMTPHelper());




            //mapper
            var config = new TypeAdapterConfig();
            config.NewConfig<User, UserDTO>()
            .Map(dest => dest.RoleNames, src => src.RoleList != null && src.RoleList.Any()
                    ? src.RoleList.Select(r => r.RoleName).ToList()
                    : new List<string>());
            config.NewConfig<Booth, BoothVO>()
                .Map(dest => dest.BrandTypeName, src => src.BrandType.BrandTypeName);

            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();


        }


    }
}
