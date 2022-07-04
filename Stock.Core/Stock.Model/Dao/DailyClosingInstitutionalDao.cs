using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Model.Dao
{
    public class DailyClosingInstitutionalDao : StockBaseDao
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string ID { get; set; }

        public int ForeignOver { get; set; }

        public int InvestmentOver { get; set; }

        public int DealerOver { get; set; }

        public int TreeInstitutionalOver { get; set; }
    }
}
