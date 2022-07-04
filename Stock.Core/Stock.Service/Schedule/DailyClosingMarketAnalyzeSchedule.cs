using Microsoft.Extensions.DependencyInjection;
using Stock.Common.Enums;
using Stock.Common.Extension;
using Stock.Common.Helpers;
using Stock.Model.BO;
using Stock.Model.Dao;
using Stock.Repository;
using Stock.Service.Scrubbing;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Stock.Service.Schedule
{
    public class DailyClosingMarketAnalyzeSchedule
    {
        public static IServiceProvider _serviceProvider { get; set; }

        public static Timer _timer { get; set; }

        public static DateTime _endTime { get; set; }

        public static DateTime _dateTime { get; set; }

        public DailyClosingMarketAnalyzeSchedule(IServiceProvider serviceProvider)
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
            var isDo = false;
            DateTime date = DateTime.Now;

            if (date.Hour >= 19 && date.Hour < 20 && date.Minute % 20 == 7)
            {
                isDo = true;
            }

            return isDo;
        }

        public static async Task Work(DateTime date, bool isMessage = false)
        {
            date = date.Date;
            var bo = await CreateData(date);
            var dao = await SaveData(bo, date);

            if (isMessage && dao.IsNotEmpty())
            {
                var message = Message(dao);
                message.AddRange(MessageCloseAdd(dao));
                foreach (var msg in message)
                {
                    await MessageHelper.SendMessageAsync(ChannelEnum.DailyReport, msg);
                }
            }
        }

        public static async Task<List<DailyClosingMarketDao>> GetOneYearData(DateTime date)
        {
            using var scope = _serviceProvider.CreateScope();
            var dailyClosingMarketRepostory = scope.ServiceProvider.GetService<DailyClosingMarketRepository>();
            var codes = await dailyClosingMarketRepostory.GetExsitCode(date);
            return await dailyClosingMarketRepostory.GetDataByOneYear(date, codes);
        }

        public static async Task<List<DailyClosingMarketDao>> GetTwoDay(DateTime date)
        {
            using var scope = _serviceProvider.CreateScope();
            var dailyClosingMarketRepostory = scope.ServiceProvider.GetService<DailyClosingMarketRepository>();
            var codes = await dailyClosingMarketRepostory.GetExsitCode(date);
            return await dailyClosingMarketRepostory.GetDataByTwoDay(date, codes);
        }

        public static async Task<List<DailyClosingMarketAnalyzeDao>> CreateData(DateTime date)
        {
            var datas = await GetOneYearData(date);
            var result = datas.OrderByDescending(r => r.DataTime).GroupBy(r => r.StockCode).Select(r =>
            {
                var list = r.Take(200).ToList();
                try
                {
                    if (!list.Any() || list.Count < 2)
                    {
                        return null;
                    }

                    var f = list[0];
                    var s = list[1];
                    if (f.DataTime != date)
                    {
                        return null;
                    }

                    return new DailyClosingMarketAnalyzeDao
                    {
                        ID = date.ToString("yyyyMMdd") + r.Key,
                        StockCode = f.StockCode,
                        StockName = f.StockName,
                        Increase = f.Increase,
                        Volume = f.Volume,
                        Close = f.Close,
                        K_Increase = ((f.Close - f.Open) * 100 / s.Close).ToFixTwo(),
                        OpenIncrease = ((f.Open - s.Close) * 100 / s.Close).ToFixTwo(),
                        VolumeIncrease = ((decimal)(f.Volume - s.Volume) * 100 / s.Volume).ToFixTwo(),
                        HigestGap = ((f.High - f.Close) * 100 / s.Close).ToFixTwo(),
                        LowestGap = ((f.Low - f.Close) * 100 / s.Close).ToFixTwo(),
                        MA3 = list.Count >= 3 ? list.Take(3).Average(r => r.Close).ToFixTwo() : null,
                        MA5 = list.Count >= 5 ? list.Take(5).Average(r => r.Close).ToFixTwo() : null,
                        MA7 = list.Count >= 7 ? list.Take(7).Average(r => r.Close).ToFixTwo() : null,
                        MA10 = list.Count >= 10 ? list.Take(10).Average(r => r.Close).ToFixTwo() : null,
                        MA15 = list.Count >= 15 ? list.Take(15).Average(r => r.Close).ToFixTwo() : null,
                        MA20 = list.Count >= 20 ? list.Take(20).Average(r => r.Close).ToFixTwo() : null,
                        MA33 = list.Count >= 33 ? list.Take(33).Average(r => r.Close).ToFixTwo() : null,
                        MA45 = list.Count >= 45 ? list.Take(45).Average(r => r.Close).ToFixTwo() : null,
                        MA60 = list.Count >= 60 ? list.Take(60).Average(r => r.Close).ToFixTwo() : null,
                        MA120 = list.Count >= 120 ? list.Take(120).Average(r => r.Close).ToFixTwo() : null,
                        MA200 = list.Count >= 200 ? list.Take(200).Average(r => r.Close).ToFixTwo() : null,
                        Add3 = list.Count >= 3 ? ((list[0].Close - list[2].Close) * 100 / list[2].Close).ToFixTwo() : null,
                        Add5 = list.Count >= 5 ? ((list[0].Close - list[4].Close) * 100 / list[4].Close).ToFixTwo() : null,
                        Add10 = list.Count >= 10 ? ((list[0].Close - list[9].Close) * 100 / list[9].Close).ToFixTwo() : null,
                        Add20 = list.Count >= 20 ? ((list[0].Close - list[19].Close) * 100 / list[19].Close).ToFixTwo() : null,
                        Add60 = list.Count >= 60 ? ((list[0].Close - list[59].Close) * 100 / list[59].Close).ToFixTwo() : null,
                        Add120 = list.Count >= 120 ? ((list[0].Close - list[119].Close) * 100 / list[119].Close).ToFixTwo() : null,
                        Add200 = list.Count >= 200 ? ((list[0].Close - list[199].Close) * 100 / list[199].Close).ToFixTwo() : null,
                        DataTime = f.DataTime,
                        CreateTime = DateTime.Now,
                    };
                }
                catch (Exception ex)
                {
                    return null;
                }
            }).Where(r => r != null).ToList();

            return result;
        }

        public static List<string> Message(List<DailyClosingMarketAnalyzeDao> data)
        {
            List<string> msg = new List<string>();
            data = data.Where(r => r.Volume > 1000).ToList();
            var strong = data.Where(r => r.Increase > 3 && r.K_Increase > 3 && r.HigestGap < (decimal)1.5 && r.VolumeIncrease > 120).OrderByDescending(r => r.Increase).ToList();
            var weak = data.Where(r => r.Increase < -4 && r.K_Increase < -3 && r.LowestGap > (decimal)-1.5 && r.VolumeIncrease > 120).OrderBy(r => r.Increase).ToList();
            var jumpStong = data.Where(r => r.OpenIncrease > (decimal)3 && r.K_Increase > 3 && r.HigestGap < (decimal)1.5).ToList();

            if (strong.IsNotEmpty())
            {
                var temp = "強勢股:\r\n\r\n";
                temp += string.Join("\r\n\r\n", strong.Select(r => $"{r.StockName}({r.StockCode}) 收:{r.Close} 漲幅:{r.Increase} 量成長:{r.VolumeIncrease}"));
                msg.Add(temp);
            }

            if (weak.IsNotEmpty())
            {
                var temp = "弱勢股:\r\n\r\n";
                temp += string.Join("\r\n\r\n", weak.Select(r => $"{r.StockName}({r.StockCode}) 收:{r.Close} 漲幅:{r.Increase} 量成長:{r.VolumeIncrease}"));
                msg.Add(temp);
            }

            if (jumpStong.IsNotEmpty())
            {
                var temp = "跳漲股:\r\n\r\n";
                temp += string.Join("\r\n", jumpStong.Select(r => $"{r.StockName}({r.StockCode}) 開漲:{r.OpenIncrease} 成交量:{r.Volume}"));
                msg.Add(temp);
            }

            return msg;
        }

        public static List<string> MessageCloseAdd(List<DailyClosingMarketAnalyzeDao> data)
        {
            List<string> msg = new List<string>();
            data = data.Where(r => r.Volume > 1000).ToList();
            var add5 = data.OrderByDescending(r => r.Add5).Take(10).ToList();
            var add10 = data.OrderByDescending(r => r.Add10).Take(10).ToList();
            var add20 = data.OrderByDescending(r => r.Add20).Take(10).ToList();
            var add60 = data.OrderByDescending(r => r.Add60).Take(10).ToList();
            var add200 = data.OrderByDescending(r => r.Add200).Take(10).ToList();

            if (add5.IsNotEmpty())
            {
                var temp = "五天內漲幅:\r\n\r\n";
                temp += string.Join("\r\n\r\n", add5.Select((r, index) => $"{index + 1}.{r.StockName}({r.StockCode}) 漲幅:{r.Add5}"));
                msg.Add(temp);
            }

            if (add10.IsNotEmpty())
            {
                var temp = "十天內漲幅:\r\n\r\n";
                temp += string.Join("\r\n\r\n", add10.Select((r, index) => $"{index + 1}.{r.StockName}({r.StockCode}) 漲幅:{r.Add10}"));
                msg.Add(temp);
            }

            if (add20.IsNotEmpty())
            {
                var temp = "月漲幅:\r\n\r\n";
                temp += string.Join("\r\n\r\n", add20.Select((r, index) => $"{index + 1}.{r.StockName}({r.StockCode}) 漲幅:{r.Add20}"));
                msg.Add(temp);
            }

            if (add60.IsNotEmpty())
            {
                var temp = "季漲福:\r\n\r\n";
                temp += string.Join("\r\n\r\n", add60.Select((r, index) => $"{index + 1}.{r.StockName}({r.StockCode}) 漲幅:{r.Add60}"));
                msg.Add(temp);
            }

            if (add200.IsNotEmpty())
            {
                var temp = "年漲福:\r\n\r\n";
                temp += string.Join("\r\n\r\n", add200.Select((r, index) => $"{index + 1}.{r.StockName}({r.StockCode}) 漲幅:{r.Add200}"));
                msg.Add(temp);
            }

            return msg;
        }

        public static async Task<List<DailyClosingMarketAnalyzeDao>> SaveData(List<DailyClosingMarketAnalyzeDao> dao, DateTime date)
        {
            using var scope = _serviceProvider.CreateScope();
            var dailyClosingMarketRepostory = scope.ServiceProvider.GetService<DailyClosingMarketAnalyzeRepository>();
            var codes = await scope.ServiceProvider.GetService<StockCodeRepository>().GetExsitID();
            var exsitIDs = await dailyClosingMarketRepostory.GetExsitID(dao.Select(r => r.ID).ToList());
            dao = dao.Where(r => !exsitIDs.Contains(r.ID)).ToList();
            await dailyClosingMarketRepostory.Insert(dao);

            return dao;
        }

        public async static void UpdateHistory()
        {
            _dateTime = DateTime.Now.Date.AddYears(-1);
            _endTime = DateTime.Now.Date;
            while (_dateTime <= _endTime)
            {
                await Work(_dateTime);
                _dateTime = _dateTime.AddDays(1);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
