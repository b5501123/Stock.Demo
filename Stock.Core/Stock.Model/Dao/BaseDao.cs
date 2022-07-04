using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Model.Dao
{
    public class BaseDao
    {
        public virtual DateTime DataTime { get; set; }

        public  DateTime CreateTime { get; set; }
    }
}
