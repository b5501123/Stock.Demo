using Stock.Common.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Stock.Model.Dao
{
    public class TimelyStockPriceDao
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string ID { get; set; }

        [JsonPropertyName("code")]
        public string StockCode { get; set; }
        public string StockName { get; set; }

        [JsonPropertyName("close")]
        public decimal Close { get; set; }
        [NotMapped]
        public decimal Y_Close { get; set; }

        [JsonPropertyName("open")]
        public decimal Open { get; set; }

        [JsonPropertyName("high")]
        public decimal High { get; set; }

        [JsonPropertyName("low")]
        public decimal Low { get; set; }

        public decimal Increase { get; set; }
        [JsonPropertyName("volume")]
        public int Volume { get; set; }

        public decimal VolumeIncrease { get; set; }
        [JsonPropertyName("time")]
        public DateTime DataTime { get; set; }

        public DateTime CreateTime { get; set; }


        public TimelyStockPriceDao ConvertData(DailyClosingMarketDao dailyClosingMarketDao)
        {
            ID = $"{DataTime.ToString("yyyyMMddHHmmss")}{StockCode}";
            StockName = dailyClosingMarketDao.StockName;
            Volume = dailyClosingMarketDao.Volume;
            Y_Close = dailyClosingMarketDao.Close;
            VolumeIncrease = ((Volume - dailyClosingMarketDao.Volume) * 100 / (decimal)dailyClosingMarketDao.Volume).ToFixTwo();
            Increase = ((Close - dailyClosingMarketDao.Close) / dailyClosingMarketDao.Close * 100).ToFixTwo();
            CreateTime = DateTime.Now;
            return this;
        }
    }
}
