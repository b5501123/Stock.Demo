using Microsoft.Extensions.DependencyInjection;
using Stock.Common.Enums;
using Stock.Common.Extension;
using Stock.Common.Helpers;
using Stock.Model.BO;
using Stock.Model.Cache;
using Stock.Model.Dao;
using Stock.Repository;
using Stock.Service.Scrubbing;
using Stock.Service.Strategy;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Stock.Service.Schedule
{
    public class MothIncomeSchedule
    {
        public static IServiceProvider _serviceProvider { get; set; }

        public static Timer _timer { get; set; }

        public static DateTime _endTime { get; set; }

        public static DateTime _dateTime { get; set; }

        public MothIncomeSchedule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public static async void JobInvoke(object? sender, ElapsedEventArgs e)
        {
            if (IsDo())
            {
                var time = DateTime.Now.Date.AddMonths(-1);
                await Work(time,true);
            }
        }

        public static bool IsDo()
        {
            var time = DateTime.Now;
            var isDo = false;
            if (time.Hour >= 8 && time.Hour < 23)
            {
                if (time.Hour >= 9 && time.Hour <= 14 && time.Minute % 5 == 0)
                {
                    isDo = true;
                }
                else if (time.Minute % 20 == 0)
                    isDo = true;
            }

            return isDo;
        }

        public static async Task Work(DateTime date, bool isMessage = false)
        {
            date = new DateTime(date.Year, date.Month, 1);
            var bo = await MonthincomeScrubbing.WorkFromDate(date);
            var daos = await SaveData(bo, date);

            if (isMessage && daos.IsNotEmpty())
            {
                var list = await GetCache(daos, date);
                var message = Message(list, date);
                await MessageHelper.SendMessageAsync(ChannelEnum.MonthIncome, message);
            }
        }

        public static async Task<List<MothIncomeDao>> SaveData(List<MonthIncomeBO> monthIncomeBO, DateTime date)
        {
            using var scope = _serviceProvider.CreateScope();
            var monthIncomeRepostory = scope.ServiceProvider.GetService<MonthIncomeRepository>();
            var codes = await scope.ServiceProvider.GetService<StockCodeRepository>().GetExsitID();
            var exsitIDs = await monthIncomeRepostory.GetExsitID(date);

            var dao = monthIncomeBO.Select(r => r.BuildDao()).ToList();

            dao = dao.Where(r => !exsitIDs.Contains(r.ID) && codes.Contains(r.StockCode)).ToList();
            await monthIncomeRepostory.Insert(dao);

            return dao;
        }

        public static void UpdateHistory()
        {
            _timer = new Timer(10 * 1000);
            _timer.Elapsed += ElapsedEventHandler;
            _dateTime = new DateTime(2018, 09, 1);
            _endTime = new DateTime(2022, 5, 1);
            _timer.Start();
        }

        public static async void ElapsedEventHandler(object? sender, ElapsedEventArgs e)
        {
            await Work(_dateTime);
            _dateTime = _dateTime.AddMonths(1);
            if (_dateTime > _endTime)
            {
                _timer.Stop();
            }
        }

        public static async Task<List<MonthIncomeCache>> GetCache(List<MothIncomeDao> mothIncomeDaos, DateTime dateTime)
        {
            using var scope = _serviceProvider.CreateScope();
            var monthIncomeRepostory = scope.ServiceProvider.GetService<MonthIncomeRepository>();
            var stockCodes = mothIncomeDaos.Select(r => r.StockCode).ToList();
            var daos = await monthIncomeRepostory.GetList(dateTime.AddYears(-5), stockCodes);

            var result = daos.GroupBy(r => r.StockCode).Select(r => CreateCache(r.ToList())).ToList();

            return result;
        }

        public static MonthIncomeCache CreateCache(List<MothIncomeDao> mothIncomeDaos)
        {
            var daos = mothIncomeDaos.OrderByDescending(r => r.DataTime).ToList();
            var f = daos[0];

            return new MonthIncomeCache
            {
                StockCode = f.StockCode,
                StockName = f.StockName,
                YoY = f.YearAdd,
                MoM = f.MonthAdd,
                AllYoY = f.CumulativeAdd,
                Remark = f.Remark,
                YoYOver100Count = BasicStrategy.Over100Count(daos),
                IsMoMover3Month = BasicStrategy.MoMover3Month(daos),
                IsHighest = BasicStrategy.IsHighest(daos),
                IsSecond = BasicStrategy.IsSecond(daos),
                DataTime = f.DataTime,
                CreateTime = f.CreateTime,
            };
        }

        public static string Message(List<MonthIncomeCache> data, DateTime date)
        {
            if (data.IsEmptyOrNull())
                return null;

            string content = "";

            var yoy_50 = data.OrderByDescending(r => r.YoY).Where(r => r.YoY > 50).Select(r =>
            {
                var msg = $"{r.StockName}({r.StockCode}) YoY:{r.YoY}% \r\nMoM:{r.MoM}% AllYoY:{r.AllYoY}%";
                return msg;
            }).ToList();

            var mom3 = data.OrderByDescending(r => r.YoY).Where(r => r.IsMoMover3Month).Select(r =>
            {
                var msg = $"{r.StockName}({r.StockCode})";
                return msg;
            }).ToList();

            var yoy100 = data.OrderByDescending(r => r.YoY).Where(r => r.Is3MonthYoY100).Select(r =>
            {
                var msg = $"{r.StockName}({r.StockCode}) 連續{r.YoYOver100Count}月";
                return msg;
            }).ToList();

            var highest = data.OrderByDescending(r => r.YoY).Where(r => r.IsHighest).Select(r =>
            {
                var msg = $"{r.StockName}({r.StockCode})";
                return msg;
            }).ToList();

            var second = data.OrderByDescending(r => r.YoY).Where(r => r.IsSecond).Select(r =>
            {
                var msg = $"{r.StockName}({r.StockCode})";
                return msg;
            }).ToList();

            if (yoy_50.IsNotEmpty())
            {
                content += $"公司{date.Month}月營收:\r\n";
                content += string.Join("\r\n\r\n", yoy_50);
            }
            if (mom3.IsNotEmpty())
            {
                content += $"\r\n.\r\n連續三月MoM成長 \r\n";
                content += string.Join("\r\n", mom3);
            }
            if (highest.IsNotEmpty())
            {
                content += $"\r\n.\r\n 營收破新高 \r\n";
                content += string.Join("\r\n", highest);
            }
            if (second.IsNotEmpty())
            {
                content += $"\r\n.\r\n營收破次高 \r\n";
                content += string.Join("\r\n", second);
            }
            if (yoy100.IsNotEmpty())
            {
                content += $"\r\n.\r\n連續3月以上YoY超過100% \r\n";
                content += string.Join("\r\n", yoy100);
            }

            if (content.IsEmptyOrNull())
                return content;

            string message = content + $"\r\n\r\n僅供參考，投資有風險，須謹慎小心";

            return message;

        }
    }
}
