using BoothHolder.Common.Configration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace BoothHolder.Extensions.ServiceExtensions
{
    public static class RedisSetup
    {
        public static void AddRedis(this IServiceCollection services)
        {
            var isRedis = Convert.ToBoolean(AppSettings.app("Redis:Enable"));
            if (isRedis)
            {
                string redisConnectionString = AppSettings.app("Redis:ConnectionString") ?? "localhost:6379";

                services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
                Console.WriteLine();
            }
            else
            {
                services.AddMemoryCache();
                services.AddDistributedMemoryCache();
            }
        }
    }
}
