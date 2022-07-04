using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Model.Dao
{
    public class StockBaseDao : BaseDao
    {
        public string StockCode { get; set; }

        public string StockName { get; set; }
    }
}
