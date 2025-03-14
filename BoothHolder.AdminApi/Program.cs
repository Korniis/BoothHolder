using BoothHolder.Common.Configration;
using BoothHolder.Common.Filter;
using BoothHolder.Extensions.Buliderextensions;
using BoothHolder.Extensions.ServiceExtensions;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BoothHolder.AdminApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
        

            var builder = WebApplication.CreateBuilder(args);
            // 配置 Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();
            // 添加服务
            builder.Services.AddConfiguration(builder);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddCustomServices(); // 自定义服务注册

            // 配置数据库、身份验证、Redis
            builder.Services.AddDbSetup();
            builder.Services.AddAuth();
            builder.Services.AddRedis();

            // 配置 CORS
            var urls = builder.Configuration.GetSection("CorsUrls").Get<string[]>();
            builder.Services.AddCors(options =>
                options.AddDefaultPolicy(builder => builder.WithOrigins(urls)
                    .AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

            // 配置全局过滤器
            builder.Services.Configure<MvcOptions>(opt =>
            {
                opt.Filters.Add<TokenAuthorizationFilter>();
            });

            var app = builder.Build();

            // 配置中间件
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.InitializeDatabase();
            }

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseAuthentication(); // 认证
            app.UseAuthorization();  // 授权
            app.MapControllers();

            app.Run();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddServices();
            services.AddRepositorits();
            return services;
        }
    }
}