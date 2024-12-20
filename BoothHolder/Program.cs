using BoothHolder.Common.Configration;
using BoothHolder.Extensions.Buliderextensions;
using BoothHolder.Extensions.ServiceExtensions;
using BoothHolder.Filter;
using Microsoft.AspNetCore.Mvc;
using Serilog;
namespace BoothHolder
{//利用某高级程序设计语言（如.NET或JAVA等）或网络开发工具（如ASP、JSP等），结合SQL Server或 MySQL等数据库知识及操作，设计一家居商场摊位招商宣介平台，具体功能模块主要包括：
 //1、信息输入、输出：能够以友好的界面形式实现摊位信息、商户信息的输入、存储、格式输出。
 //2、查询模块：主要实现多种方式的信息查询，比如按摊位名称/位置、按品牌类型、按招租日期等查询。
 //3、计算及统计模块，比如营业额、收益率等计算。
 //4、适当实现活动预约等其它功能。

    public class Program 
    {
        public static void Main(string[] args)
        {
            // 配置 Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // 设置最小日志级别为 Debug
                .WriteTo.Console() // 输出日志到控制台

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

                opt.Filters.Add<TokenAuthorizationFilter>();
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
            app.UseAuthentication();   //认证

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
