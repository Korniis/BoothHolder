using BoothHolder.Common.Configration;
using BoothHolder.Common.Filter;
using BoothHolder.Extensions.Buliderextensions;
using BoothHolder.Extensions.ServiceExtensions;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BoothHolder.EnterpriseApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ���� Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // ������С��־����Ϊ Debug
                .WriteTo.Console() // �����־������̨

                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.



            //settings

            builder.Services.AddConfiguration(builder);
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();


            //jwt

            // Service

            builder.Services.AddServices();

            //Repository
            builder.Services.AddRepositorits();


            Log.Debug(AppSettings.app("Database:MySql:ConnectionString"));
            Console.WriteLine(AppSettings.app<string>("JwtSettings"));
            builder.Services.AddDbSetup();
            builder.Services.AddAuth();
            builder.Services.AddRedis();

            builder.Services.Configure<MvcOptions>(opt =>
            {

                opt.Filters.Add<EnterpriseTokenFilter>();
            });
            var urls = builder.Configuration.GetSection("CorsUrls").Get<string[]>();
            builder.Services.AddCors(options =>
                options.AddDefaultPolicy(builder => builder.WithOrigins(urls)
                .AllowAnyMethod().AllowAnyHeader().AllowCredentials()));




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.InitializeDatabase();

            }
            app.UseCors();
            app.UseHttpsRedirection();
            app.UseAuthentication();   //��֤

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
