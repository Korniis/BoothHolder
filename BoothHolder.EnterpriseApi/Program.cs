
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

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
