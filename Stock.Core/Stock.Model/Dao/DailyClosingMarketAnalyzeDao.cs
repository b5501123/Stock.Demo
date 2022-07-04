using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Model.Dao
{
    public class DailyClosingMarketAnalyzeDao : StockBaseDao
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string ID { get; set; }
        public decimal Close { get; set; }
        public decimal K_Increase { get; set; }
        public decimal Increase { get; set; }
        public decimal OpenIncrease { get; set; }
        public int Volume { get; set; }

        public decimal VolumeIncrease { get; set; }
        public decimal HigestGap { get; set; }
        public decimal LowestGap { get; set; }
        public decimal? MA3 { get; set; }
        public decimal? MA5 { get; set; }
        public decimal? MA7 { get; set; }
        public decimal? MA10 { get; set; }
        public decimal? MA15 { get; set; }
        public decimal? MA20 { get; set; }
        public decimal? MA33 { get; set; }
        public decimal? MA45 { get; set; }
        public decimal? MA60 { get; set; }
        public decimal? MA120 { get; set; }
        public decimal? MA200 { get; set; }
        public decimal? Add3 { get; set; }
        public decimal? Add5 { get; set; }
        public decimal? Add10 { get; set; }
        public decimal? Add20 { get; set; }
        public decimal? Add60 { get; set; }
        public decimal? Add120 { get; set; }
        public decimal? Add200 { get; set; }
    }
}
