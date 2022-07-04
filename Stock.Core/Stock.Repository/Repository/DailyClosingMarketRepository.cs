using Dapper;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Stock.Common.Extension;
using Stock.Common.Helpers;
using Stock.DB;
using Stock.Model.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Repository
{
    public class DailyClosingMarketRepository : BaseRepository
    {
        public DailyClosingMarketRepository(StockDbContext db) : base(db)
        {
        }

        public async Task<bool> Insert(List<DailyClosingMarketDao> daos)
        {
            await _db.DailyClosingMarket.AddRangeAsync(daos);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<DailyClosingMarketDao>> GetDataByOneYear(DateTime dateTime, List<string> stockCodes = null)
        {
            var query = _db.DailyClosingMarket.AsQueryable();

            if (stockCodes.IsNotEmpty())
            {
                query = query.Where(r => stockCodes.Contains(r.StockCode));
            }

            return await query
                .Where(r => r.DataTime >= dateTime.AddYears(-1) && r.DataTime <= dateTime)
                .Select(r => new DailyClosingMarketDao
                {
                    StockCode = r.StockCode,
                    StockName = r.StockName,
                    Open = r.Open,
                    High = r.High,
                    Low = r.Low,
                    Close = r.Close,
                    Increase = r.Increase,
                    Volume = r.Volume,
                    DataTime = r.DataTime,
                }).ToListAsync();
        }

        public async Task<List<DailyClosingMarketDao>> GetDataByTwoDay(DateTime dateTime, List<string> stockCodes = null)
        {
            var query = _db.DailyClosingMarket.AsQueryable();

            if (stockCodes != null)
            {
                query = query.Where(r => stockCodes.Contains(r.StockCode));
            }

            return await query
                .Where(r => r.DataTime >= dateTime.AddDays(-1) && r.DataTime <= dateTime).OrderByDescending(r => r.DataTime)
                .Select(r => new DailyClosingMarketDao
                {
                    StockCode = r.StockCode,
                    Open = r.Open,
                    High = r.High,
                    Low = r.Low,
                    Close = r.Close,
                    Increase = r.Increase,
                    Volume = r.Volume,
                    DataTime = r.DataTime,
                }).ToListAsync();
        }

        public async Task<List<string>> GetExsitID(DateTime time)
        {
            return await _db.DailyClosingMarket.Where(r => r.DataTime == time).Select(r => r.ID).ToListAsync();
        }

        public async Task<List<string>> GetExsitCode(DateTime time)
        {
            return await _db.DailyClosingMarket.Where(r => r.DataTime == time).Select(r => r.StockCode).ToListAsync();
        }

        public async Task<List<DailyClosingMarketDao>> GetLastCloseGroupbyStockCode()
        {
            using var conn = new MySqlConnection(ConfigHelper.MySqlConnectionString);
            var dateTime = DateTime.Now.AddMonths(-1);
            var sql = @$"
SELECT {GetBasicColumnName<DailyClosingMarketDao>()}
FROM Stock.DailyClosingMarket 
WHERE `ID` IN 
(
SELECT MAX(`ID`) 
FROM Stock.DailyClosingMarket 
where `DataTime` > @dateTime 
GROUP BY `StockCode`
)";

            await conn.OpenAsync();
            var data = await conn.QueryAsync<DailyClosingMarketDao>(sql, new { dateTime });

            return data.ToList();
        }
    }
}
