using Microsoft.Extensions.Configuration;
using System.Text;

namespace Stock.Common.Helpers
{
    public class ConfigHelper //MS950
    {
        public ConfigHelper(IConfiguration configuration)
        {
            MySqlConnectionString = configuration.GetConnectionString("MySql");
            TelegramBotApi = configuration.GetConnectionString("Telegram");
            RedisConnectionString = configuration.GetConnectionString("Redis");
        }

        public static string MySqlConnectionString { get; set; }

        public static string TelegramBotApi { get; set; }

        public static string RedisConnectionString { get; set; }
    }
}
