using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Text;

namespace Stock.Common.Helpers
{
    public class RedisHelper //MS950
    {
        private static IDatabase _client;
        public RedisHelper()
        {
            _client = ConnectionMultiplexer.Connect(ConfigHelper.RedisConnectionString).GetDatabase();
        }

        public static IDatabase GetClient()
        {
            return _client;
        }
    }
}
