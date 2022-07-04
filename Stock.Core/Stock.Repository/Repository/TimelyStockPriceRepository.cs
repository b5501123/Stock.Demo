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
    public class TimelyStockPriceRepository : BaseRepository
    {
        public TimelyStockPriceRepository(StockDbContext db) : base(db)
        {
        }

        public async Task<bool> Insert(List<TimelyStockPriceDao> daos)
        {
            await _db.TimelyStockPrice.AddRangeAsync(daos);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> GetExsitID(DateTime time)
        {
            return await _db.StockNews.Where(r => r.DataTime > time.Date).Select(r => r.ID).ToListAsync();
        }

        public async Task<List<TimelyStockPriceDao>> GetTodayData(List<string> stockCode)
        {
            var date = DateTime.Now.Date;

            return await _db.TimelyStockPrice
                .Where(r => r.DataTime > date && stockCode.Contains(r.StockCode))
                .OrderByDescending(r=>r.DataTime).ToListAsync();
        }
    }
}
