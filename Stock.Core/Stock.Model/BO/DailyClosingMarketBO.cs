using Stock.Common.Extension;
using Stock.Model.Dao;

namespace Stock.Model.BO
{
    public class DailyClosingMarketBO : BaseStockBO
    {
        public decimal Open { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Close { get; set; }

        public decimal UpDown { get; set; }

        public decimal Increase { get; set; }

        public decimal Amplitude { get; set; }

        public int Volume { get; set; }

        public int Count { get; set; }

        public decimal AverageSheet { get; set; }

        public long DealMoney { get; set; }

        public DailyClosingMarketDao BuildDao()
        {
            var dao = this.Reflect<DailyClosingMarketDao>();
            dao.ID = this.DataTime.ToString("yyyyMMdd") + dao.StockCode;
            return dao;
        }
    }
}
