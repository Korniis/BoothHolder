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
            // ���� Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();
            // ��ӷ���
            builder.Services.AddConfiguration(builder);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddCustomServices(); // �Զ������ע��

            // �������ݿ⡢�����֤��Redis
            builder.Services.AddDbSetup();
            builder.Services.AddAuth();
            builder.Services.AddRedis();

            // ���� CORS
            var urls = builder.Configuration.GetSection("CorsUrls").Get<string[]>();
            builder.Services.AddCors(options =>
                options.AddDefaultPolicy(builder => builder.WithOrigins(urls)
                    .AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

            // ����ȫ�ֹ�����
            builder.Services.Configure<MvcOptions>(opt =>
            {
                opt.Filters.Add<TokenAuthorizationFilter>();
            });

            var app = builder.Build();

            // �����м��
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.InitializeDatabase();
            }

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseAuthentication(); // ��֤
            app.UseAuthorization();  // ��Ȩ
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