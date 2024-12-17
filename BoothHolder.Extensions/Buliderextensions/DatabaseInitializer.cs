using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System.Reflection;

namespace BoothHolder.Extensions.Buliderextensions
{
    public static class DatabaseInitializer
    {

        public static void InitializeDatabase(this WebApplication webApplication)
        {
            if (webApplication == null) throw new ArgumentNullException();

            var sqlSugar = webApplication.Services.GetService<ISqlSugarClient>();

            try
            {
                Console.WriteLine("正在初始化数据库...");

                // 创建数据库（如果不存在）
                sqlSugar.DbMaintenance.CreateDatabase();
                Console.WriteLine("数据库创建完成（如已存在则跳过）。");

                // 获取需要同步的实体类
                Type[] types = Assembly
                    .LoadFrom("./bin/Debug/net8.0/BoothHolder.Model.dll") // 加载指定程序集，注意路径和文件名要正确
                    .GetTypes() // 获取程序集中的所有类型
                    .Where(it => it.Namespace != null && it.Namespace.Contains("BoothHolder.Model.Entity")) // 根据命名空间过滤
                    .ToArray(); // 转换为数组

                // 如果没有找到任何符合条件的类型，抛出异常
                if (types.Length == 0)
                {
                    Console.WriteLine("没有找到匹配的实体类型");
                    return;
                }
                //



                sqlSugar.CodeFirst.SetStringDefaultLength(200).InitTables(types);
                Console.WriteLine("表结构同步完成。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"数据库初始化失败：{ex.Message}");
                throw;
            }
        }


    }
}
