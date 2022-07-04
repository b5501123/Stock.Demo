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
    public class MonthIncomeRepository : BaseRepository
    {
        public MonthIncomeRepository(StockDbContext db) : base(db)
        {
        }

        public async Task<bool> Insert(List<MothIncomeDao> daos)
        {
            await _db.MothIncome.AddRangeAsync(daos);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<MothIncomeDao>> GetList(DateTime startTime, List<string> stockeCodes = null)
        {
            var query = _db.MothIncome.AsNoTracking().AsQueryable();

            if (stockeCodes != null)
            {
                query = query.Where(r => stockeCodes.Contains(r.StockCode));
            }

            return await query.Where(r => r.DataTime >= startTime)
                .OrderByDescending(r => r.DataTime)
                .ToListAsync();

        }

        public async Task<List<string>> GetExsitID(DateTime time)
        {
            return await _db.MothIncome.Where(r => r.DataTime == time).Select(r => r.ID).ToListAsync();
        }
    }
}
