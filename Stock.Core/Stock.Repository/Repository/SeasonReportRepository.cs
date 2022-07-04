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
    public class SeasonReportRepository : BaseRepository
    {
        public SeasonReportRepository(StockDbContext db) : base(db)
        {
        }

        public async Task<bool> Insert(List<SeasonReportDao> daos)
        {
            await _db.SeasonReport.AddRangeAsync(daos);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> GetExsitID(DateTime time)
        {
            return await _db.SeasonReport.Where(r => r.DataTime == time).Select(r => r.ID).ToListAsync();
        }
    }
}
