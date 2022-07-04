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
    public class StockCodeRepository : BaseRepository
    {
        public StockCodeRepository(StockDbContext db) : base(db)
        {
        }

        public async Task<bool> Insert(List<StockCodeDao> daos)
        {
            await _db.StockCode.AddRangeAsync(daos);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> GetExsitID()
        {
            return await _db.StockCode.Select(r => r.StockCode).ToListAsync();
        }
    }
}
