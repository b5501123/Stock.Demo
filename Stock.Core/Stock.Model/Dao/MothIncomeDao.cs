using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Model.Dao
{
    public class MothIncomeDao : StockBaseDao
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string ID { get; set; }

        public long CurrentIncome { get; set; }

        public long LastIncome { get; set; }

        public long LY_CurrentIncome { get; set; }

        public decimal MonthAdd { get; set; }

        public decimal YearAdd { get; set; }

        public long CumulativeIncome { get; set; }

        public long LY_CumulativeIncome { get; set; }

        public decimal CumulativeAdd { get; set; }

        public string Remark { get; set; }
    }
}
