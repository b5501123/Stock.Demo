using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Model.Dao
{
    public class SeasonReportDao : StockBaseDao
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string ID { get; set; }

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
    }
}
