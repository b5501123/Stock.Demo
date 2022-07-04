using Microsoft.EntityFrameworkCore;
using Stock.DB;
using Stock.Model.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Repository
{
    public class StockNewsRepository : BaseRepository
    {
        public StockNewsRepository(StockDbContext db) : base(db)
        {
        }

        public async Task<bool> Insert(List<StockNewsDao> daos)
        {
            await _db.StockNews.AddRangeAsync(daos);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> GetExsitID(DateTime time)
        {
            return await _db.StockNews.Where(r => r.DataTime > time.AddDays(-3)).Select(r => r.ID).ToListAsync();
        }
    }
}
