using Stock.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Repository
{
    public  abstract class BaseRepository
    {
        public StockDbContext _db { get; }

        public BaseRepository(StockDbContext db)
        {
            _db = db;
        }

        public static string GetBasicColumnName<T>(string appendPrefix = "")
        {
            var type = typeof(T);
            var names = type.GetProperties().Select(a => appendPrefix + "`" + a.Name + "`")
                .ToList();
            return string.Join(",", names);
        }
    }
}
