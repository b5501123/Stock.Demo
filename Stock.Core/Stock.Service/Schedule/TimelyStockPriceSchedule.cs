using Microsoft.Extensions.DependencyInjection;
using Stock.Common.Enums;
using Stock.Common.Extension;
using Stock.Common.Helpers;
using Stock.Model.BO;
using Stock.Model.Cache;
using Stock.Model.Dao;
using Stock.Repository;
using Stock.Service.Scrubbing;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Stock.Service.Schedule
{
    public class TimelyStockPriceSchedule
    {
        public static IServiceProvider _serviceProvider { get; set; }

        public static Timer _timer { get; set; }

        public static DateTime _endTime { get; set; }

        public static DateTime _dateTime { get; set; }

        public TimelyStockPriceSchedule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static async void JobInvoke(object? sender, ElapsedEventArgs e)
        {
            if (IsDo())
            {
                var time = DateTime.Now.Date;
                await Work(time, true);
            }
        }

        public static bool IsDo()
        {
            DateTime date = DateTime.Now;
            DateTime start = DateTime.Now.Date.AddHours(9);
            DateTime end = DateTime.Now.Date.AddHours(13).AddMinutes(30);

            return date >= start && date <= end;
        }

        public static async Task Work(DateTime date, bool isMessage = false)
        {
            date = date.Date;
            var data = await TimelyStockPriceScrubbing.GetData();
            var daos = await SaveData(data, date);
            if (daos.IsNotEmpty())
            {
                var cache = await CreateCache(daos.Select(r => r.StockCode).ToList(),date);

                var upData = cache.Where(r => r.Increase_5 > 2).ToList();
                var limitData = cache.Where(r => r.IsFirst7 || r.IsFirst8 || r.IsFirst9).ToList();
                if(upData.IsNotEmpty())
                {
                    var msg =RiseMessage(upData);
                    await MessageHelper.SendMessageAsync(ChannelEnum.Rise, msg);
                }


                if (limitData.IsNotEmpty())
                {
                    var msg = DailyLimitMessage(limitData);
                    await MessageHelper.SendMessageAsync(ChannelEnum.DailyLimit, msg);
                }
            }
        }

        public static async Task<List<TimelyStockPriceDao>> SaveData(List<TimelyStockPriceDao> timelyStockPriceDao, DateTime date)
        {
            using var scope = _serviceProvider.CreateScope();
            var dailyClosingMarketRepository = scope.ServiceProvider.GetService<DailyClosingMarketRepository>();
            var timelyStockPriceRepository = scope.ServiceProvider.GetService<TimelyStockPriceRepository>();
            var lastStockClose = await dailyClosingMarketRepository.GetLastCloseGroupbyStockCode();


            var codes = await scope.ServiceProvider.GetService<StockCodeRepository>().GetExsitID();
            var exsitIDs = await timelyStockPriceRepository.GetExsitID(date);

            var dao = timelyStockPriceDao.Where(r => !exsitIDs.Contains(r.ID) && codes.Contains(r.StockCode)).Select(r =>
            {
                var last = lastStockClose.FirstOrDefault(k => k.StockCode == r.StockCode);
                if (last == null)
                {
                    return null;
                }

                return r.ConvertData(last);
            }).Where(r => r != null).ToList();

            await timelyStockPriceRepository.Insert(dao);

            return dao;
        }

        public static async Task<List<UpcomingDailyLimitCache>> CreateCache(List<string> stockCode, DateTime time)
        {
            using var scope = _serviceProvider.CreateScope();
            var timelyStockPriceRepository = scope.ServiceProvider.GetService<TimelyStockPriceRepository>();

            var timelyStockPriceDaos = await timelyStockPriceRepository.GetTodayData(stockCode);

            var result = timelyStockPriceDaos.GroupBy(r => r.StockCode)
                .Select(r =>
                {
                    var list = r.OrderByDescending(o => o.DataTime).ToList();
                    var item = list.FirstOrDefault();
                    var min = list.Where(r => r.DataTime >= time.AddMinutes(-5)).OrderBy(r => r.Close).FirstOrDefault();
                    var cache = new UpcomingDailyLimitCache
                    {
                        StockCode = item.StockCode,
                        StockName = item.StockName,
                        Deal = item.Close,
                        Increase = item.Increase,
                        Volume = item.Volume,
                        VolumeIncrease = item.VolumeIncrease,
                        IsFirst7 = item.Increase >= 7 && list.Count(r => r.Increase >= 7) == 1,
                        IsFirst8 = item.Increase >= 8 && list.Count(r => r.Increase >= 8) == 1,
                        IsFirst9 = item.Increase >= 9 && list.Count(r => r.Increase >= 9) == 1,
                    };
                    if (min != null)
                    {
                        cache.Increase_5 = ((item.Close - min.Close) * 100 / item.Y_Close).ToFixTwo();
                    }
                    return cache;
                }).ToList();

            return result.Where(r => r.Volume > 1000).ToList();
        }

        public static string RiseMessage(List<UpcomingDailyLimitCache> data)
        {
            var msg = $"上漲報\r\n\r\n";

            var list = data.OrderByDescending(r => r.Increase).Select(r =>
            {
                return $"{r.StockName} [({r.StockCode})](https://tw.stock.yahoo.com/q/bc?s={r.StockCode}) 成交價:{r.Deal} ({r.Increase}%) 5分內上漲{r.Increase_5}%  成交量成長:{r.VolumeIncrease}%";
            }).ToList();

            msg += string.Join("\r\n\r\n", list) + $"\r\n\r\n僅供參考，投資有風險，須謹慎小心";

            return msg;
        }

        public static string DailyLimitMessage(List<UpcomingDailyLimitCache> data)
        {
            var msg = $"漲停報\r\n\r\n";

            var list = data.OrderByDescending(r => r.Increase).Select(r =>
            {
                return $"{r.StockName} [({r.StockCode})](https://tw.stock.yahoo.com/q/bc?s={r.StockCode}) 成交價:{r.Deal} ({r.Increase}%)  成交量成長:{r.VolumeIncrease}%";
            }).ToList();

            msg += string.Join("\r\n\r\n", list) + $"\r\n\r\n僅供參考，投資有風險，須謹慎小心";

            return msg;
        }



        public static List<string> Message(List<StockNewsDao> data)
        {
            var msg = data.Where(r => r.Type == StockNewsEnum.SelfAssessment).Select(r => $"{r.StockName}({r.StockCode})\r\n標題:{r.Titile}\r\n\r\n內容:{r.Content}").ToList();

            return msg;
        }


    }
}
