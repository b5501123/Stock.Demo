using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Model.BO
{
    public class BaseStockBO
    {
        public string StockCode { get; set; }

        public string StockName { get; set; }

        public DateTime DataTime { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
