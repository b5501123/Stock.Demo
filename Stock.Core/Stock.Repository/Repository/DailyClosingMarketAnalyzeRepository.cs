using Microsoft.EntityFrameworkCore;
using Stock.Common.Extension;
using Stock.DB;
using Stock.Model.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Repository
{
    public class DailyClosingMarketAnalyzeRepository : BaseRepository
    {
        public DailyClosingMarketAnalyzeRepository(StockDbContext db) : base(db)
        {
        }

        public async Task<bool> Insert(List<DailyClosingMarketAnalyzeDao> daos)
        {
            await _db.DailyClosingMarketAnalyze.AddRangeAsync(daos);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> GetExsitID(List<string> stockcode)
        {
            return await _db.DailyClosingMarketAnalyze.Where(r => stockcode.Contains(r.ID)).Select(r => r.ID).ToListAsync();
        }
    }
}
