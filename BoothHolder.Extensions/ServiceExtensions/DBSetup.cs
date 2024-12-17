using BoothHolder.Common.Configration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SqlSugar;
using System.Reflection;

namespace BoothHolder.Extensions.ServiceExtensions
{
    public static class DBSetup
    {
        public static void AddDbSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddHttpContextAccessor();
            services.AddSingleton<ISqlSugarClient>(s =>
            {
                SqlSugarScope sqlSugar = new SqlSugarScope(new ConnectionConfig()
                {
                    DbType = SqlSugar.DbType.MySql,
                    ConnectionString = AppSettings.app("Database:MySql:ConnectionString"),
                    IsAutoCloseConnection = true,
                    ConfigureExternalServices = new ConfigureExternalServices
                    {
                        //注意:  这儿AOP设置不能少
                        EntityService = (c, p) =>
                        {


                            /***高版C#写法***/
                            //支持string?和string  
                            if (p.IsPrimarykey == false && new NullabilityInfoContext()
                             .Create(c).WriteState is NullabilityState.Nullable)
                            {
                                p.IsNullable = true;
                            }
                        }
                    }

                },


               db =>
               {
                   //每次上下文都会执行

                   //获取IOC对象不要求在一个上下文
                   //var log=s.GetService<Log>()

                   //获取IOC对象要求在一个上下文
                   //var appServive = s.GetService<IHttpContextAccessor>();
                   //var log= appServive?.HttpContext?.RequestServices.GetService<Log>();

                   db.Aop.OnLogExecuting = (sql, pars) =>
                   {

                       Log.Information($"SQL执行中: {sql}");
                   };



               });
                // 启动时执行数据库初始化操作

                return sqlSugar;
            });


            Log.Debug("chau");
        }

    }

}
