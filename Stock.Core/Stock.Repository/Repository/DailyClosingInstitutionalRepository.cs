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
    public class DailyClosingInstitutionalRepository : BaseRepository
    {
        public DailyClosingInstitutionalRepository(StockDbContext db) : base(db)
        {
        }

        public async Task<bool> Insert(List<DailyClosingInstitutionalDao> daos)
        {
            await _db.DailyClosingInstitutional.AddRangeAsync(daos);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> GetExsitID(DateTime time)
        {
            return await _db.DailyClosingInstitutional.Where(r => r.DataTime == time).Select(r => r.ID).ToListAsync();
        }
    }
}
