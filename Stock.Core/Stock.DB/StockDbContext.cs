using Microsoft.EntityFrameworkCore;
using Stock.Model.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.DB
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options)
           : base(options)
        {
        }

        public DbSet<StockCodeDao> StockCode { get; set; }

        public DbSet<MothIncomeDao> MothIncome { get; set; }

        public DbSet<DailyClosingInstitutionalDao> DailyClosingInstitutional { get; set; }

        public DbSet<DailyClosingMarketDao> DailyClosingMarket { get; set; }

        public DbSet<DailyClosingMarketAnalyzeDao> DailyClosingMarketAnalyze { get; set; }

        public DbSet<TimelyStockPriceDao> TimelyStockPrice { get; set; }

        public DbSet<SeasonReportDao> SeasonReport { get; set; }

        public DbSet<StockNewsDao> StockNews { get; set; }
    }
}
