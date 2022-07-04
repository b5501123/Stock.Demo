using HtmlAgilityPack;
using Stock.Common.Helpers;
using Stock.Model.BO;
using Stock.Model.Dao;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Stock.Service.Scrubbing
{
    public static class TimelyStockPriceScrubbing
    {
        public static async Task<List<TimelyStockPriceDao>> GetData()
        {
            var data = new List<TimelyStockPriceDao>();
            try
            {

                var client = RedisHelper.GetClient();
                var text = await client.StringGetAsync("stock_api:data");
                JsonSerializerOptions option = new JsonSerializerOptions
                {
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };
                option.Converters.Add(new DateTimeParse());

                data = JsonSerializer.Deserialize<List<TimelyStockPriceDao>>(text, option);
            }
            catch(Exception e)
            {
                return new List<TimelyStockPriceDao>();
            }

            return data;
        }

        public class DateTimeParse : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return DateTime.Parse(reader.GetString() ?? string.Empty);
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }

    }
}
