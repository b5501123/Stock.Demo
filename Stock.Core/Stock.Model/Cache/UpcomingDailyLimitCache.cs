
namespace Stock.Model.Cache
{
    public class UpcomingDailyLimitCache
    {
        public string StockCode { get; set; }
        public string StockName { get; set; }
        public decimal Deal { get; set; }
        public decimal Increase { get; set; }
        public decimal Increase_5 { get; set; }
        public decimal VolumeIncrease { get; set; }

        public int Volume { get; set; }
        public bool IsFirst7 { get; set; }
        public bool IsFirst8 { get; set; }
        public bool IsFirst9 { get; set; }
    }
}
