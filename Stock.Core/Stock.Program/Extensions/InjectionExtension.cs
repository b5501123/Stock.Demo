
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Stock.Common.Helpers;
using Stock.DB;
using System.Reflection;

namespace Stock.Program.Extensions
{
    public static class InjectionExtension
    {
        public static IServiceCollection AddStockServices(this IServiceCollection services, IConfiguration configuration)
        {
            _ = new ConfigHelper(configuration);
            _ = new RedisHelper();
            string connectionString = ConfigHelper.MySqlConnectionString;
            ServerVersion serverVersion = ServerVersion.AutoDetect(connectionString);
            services.AddDbContext<StockDbContext>(options =>
            {
                options.UseMySql(connectionString, serverVersion);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            services.InjectionServiceScope("Stock.Repository", "Repository");
            services.InjectionSchedule("Stock.Service", "Schedule");
            return services;
        }
        public static void InjectionServiceScope(this IServiceCollection services, string assemblyName, string suffixName)
        {

            var assembly = Assembly.Load(assemblyName);
            var list2 = assembly.GetTypes().ToList();
            var types = assembly.GetTypes().Where(x => x.Name.EndsWith(suffixName) && x.IsClass && !x.IsAbstract);
            var list = types.ToList();
            foreach (var s in types)
            {
                var iService = s.GetInterface($"I{s.Name}");
                if (iService != null)
                {
                    services.AddScoped(iService, s);
                }
                else
                {
                    services.AddScoped(s);
                }
            }
        }

        public static void InjectionServiceSingleton(this IServiceCollection services, string assemblyName, string suffixName)
        {

            var assembly = Assembly.Load(assemblyName);
            var types = assembly.GetTypes().Where(x => x.Name.EndsWith(suffixName) && x.IsClass && !x.IsAbstract);
            var list = types.ToList();
            foreach (var s in types)
            {
                var iService = s.GetInterface($"I{s.Name}");
                if (iService != null)
                {
                    services.AddSingleton(iService, s);
                }
                else
                {
                    services.AddSingleton(s);
                }
            }
        }

        public static void InjectionSchedule(this IServiceCollection services, string assemblyName, string suffixName)
        {

            var assembly = Assembly.Load(assemblyName);
            var types = assembly.GetTypes().Where(x => x.Name.EndsWith(suffixName) && x.IsClass && !x.IsAbstract);
            var list = types.ToList();
            var provider = services.BuildServiceProvider();
            foreach (var s in types)
            {
                Activator.CreateInstance(s, provider);
            }
        }

    }
}
