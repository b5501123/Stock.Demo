using Stock.Common.Extension;
using Stock.Model.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Model.BO
{
    public class MonthIncomeBO : BaseStockBO
    {
        public long CurrentIncome { get; set; }

        public long LastIncome { get; set; }

        public long LY_CurrentIncome { get; set; }

        public decimal MonthAdd { get; set; }

        public decimal YearAdd { get; set; }

        public long CumulativeIncome { get; set; }

        public long LY_CumulativeIncome { get; set; }

        public decimal CumulativeAdd { get; set; }

        public string Remark { get; set; }

        public MothIncomeDao BuildDao()
        {
            var dao = this.Reflect<MothIncomeDao>();
            dao.ID = this.DataTime.ToString("yyyyMMdd") + dao.StockCode;
            return dao;
        }

    }


}
