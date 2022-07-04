using Stock.Common.Extension;
using Stock.Model.Dao;

namespace Stock.Model.BO
{
    public class SeasonReportBO : BaseStockBO
    {
        public int Season { get; set; }
        /// <summary>
        /// 營業收入
        /// </summary>
        public long OperatingIncome { get; set; }

        /// <summary>
        /// 營業損益
        /// </summary>
        public long OperatingProfitLoss { get; set; }

        /// <summary>
        /// 業外收入
        /// </summary>
        public long OutSideIncome { get; set; }

        /// <summary>
        /// 稅前損益
        /// </summary>
        public long TaxBegoreProfitLoss { get; set; }

        /// <summary>
        /// 稅後損益
        /// </summary>
        public long TaxAfterProfitLoss { get; set; }

        public decimal EPS { get; set; }

        public SeasonReportDao BuildDao()
        {
            var dao = this.Reflect<SeasonReportDao>();
            dao.ID = this.DataTime.ToString("yyyyMMdd") + dao.StockCode;
            return dao;
        }
    }
}
