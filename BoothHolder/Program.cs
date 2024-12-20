using BoothHolder.Common.Configration;
using BoothHolder.Extensions.Buliderextensions;
using BoothHolder.Extensions.ServiceExtensions;
using BoothHolder.Filter;
using Microsoft.AspNetCore.Mvc;
using Serilog;
namespace BoothHolder
{//����ĳ�߼�����������ԣ���.NET��JAVA�ȣ������翪�����ߣ���ASP��JSP�ȣ������SQL Server�� MySQL�����ݿ�֪ʶ�����������һ�Ҿ��̳�̯λ��������ƽ̨�����幦��ģ����Ҫ������
 //1����Ϣ���롢������ܹ����ѺõĽ�����ʽʵ��̯λ��Ϣ���̻���Ϣ�����롢�洢����ʽ�����
 //2����ѯģ�飺��Ҫʵ�ֶ��ַ�ʽ����Ϣ��ѯ�����簴̯λ����/λ�á���Ʒ�����͡����������ڵȲ�ѯ��
 //3�����㼰ͳ��ģ�飬����Ӫҵ������ʵȼ��㡣
 //4���ʵ�ʵ�ֻԤԼ���������ܡ�

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
            app.UseAuthentication();   //��֤

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
