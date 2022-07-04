using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Stock.Common.Extension;
using Stock.Model.BO;
using Stock.Model.Dao;
using Stock.Repository;
using Stock.Service.Scrubbing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Service.Schedule
{
    public class StockCodeSchedule
    {
        public static IServiceProvider _serviceProvider { get; set; }

        public StockCodeSchedule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static async Task Work(DateTime date)
        {
            var bo = await StockCodeScrubbing.WorkFromDate(date);
            await SaveData(bo);
        }

        public static async Task SaveData(List<StockCodeBO> stockCodeBOs)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetService<StockCodeRepository>();
            List<string> exsitID = await service.GetExsitID();
            stockCodeBOs = stockCodeBOs.Where(r => !exsitID.Contains(r.StockCode)).ToList();
            var dao = stockCodeBOs.Reflect<StockCodeDao>();
            await service.Insert(dao);
        }
    }
}
