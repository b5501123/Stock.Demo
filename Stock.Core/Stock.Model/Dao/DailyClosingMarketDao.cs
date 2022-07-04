using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Model.Dao
{
    public class DailyClosingMarketDao : StockBaseDao
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string ID { get; set; }

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
    }
}
