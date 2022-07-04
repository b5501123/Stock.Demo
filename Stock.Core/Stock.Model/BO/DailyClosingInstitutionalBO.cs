using Stock.Common.Extension;
using Stock.Model.Dao;

namespace Stock.Model.BO
{
    public class DailyClosingInstitutionalBO : BaseStockBO
    {
        public int ForeignOver { get; set; }

        public int InvestmentOver { get; set; }

        public int DealerOver { get; set; }

        public int TreeInstitutionalOver { get; set; }

        public DailyClosingInstitutionalDao BuildDao()
        {
            var dao = this.Reflect<DailyClosingInstitutionalDao>();
            dao.ID = this.DataTime.ToString("yyyyMMdd") + dao.StockCode;
            return dao;
        }
    }
}
