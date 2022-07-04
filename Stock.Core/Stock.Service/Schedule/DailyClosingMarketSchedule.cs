using Microsoft.Extensions.DependencyInjection;
using Stock.Model.BO;
using Stock.Repository;
using Stock.Service.Scrubbing;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Stock.Service.Schedule
{
    public class DailyClosingMarketSchedule
    {
        public static IServiceProvider _serviceProvider { get; set; }

        public static Timer _timer { get; set; }

        public static DateTime _endTime { get; set; }

        public static DateTime _dateTime { get; set; }

        public DailyClosingMarketSchedule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static async void JobInvoke(object? sender, ElapsedEventArgs e)
        {
            if (IsDo())
            {
                var time = DateTime.Now.Date;
                await Work(time);
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

        public static async Task Work(DateTime date)
        {
            date = date.Date;
            var bo = await DailyClosingMarketScrubbing.WorkFromDate(date);
            await SaveData(bo, date);
        }

        public static async Task SaveData(List<DailyClosingMarketBO> dailyClosingMarketBO, DateTime date)
        {
            using var scope = _serviceProvider.CreateScope();
            var dailyClosingMarketRepostory = scope.ServiceProvider.GetService<DailyClosingMarketRepository>();
            var codes = await scope.ServiceProvider.GetService<StockCodeRepository>().GetExsitID();
            var exsitIDs = await dailyClosingMarketRepostory.GetExsitID(date);

            var dao = dailyClosingMarketBO.Select(r => r.BuildDao()).ToList();
            dao = dao.Where(r => r.Close > 0 && r.Volume > 0).ToList();
            dao = dao.Where(r => !exsitIDs.Contains(r.ID) && codes.Contains(r.StockCode)).ToList();
            await dailyClosingMarketRepostory.Insert(dao);
        }

        public static void UpdateHistory()
        {
            _timer = new Timer(10 * 1000);
            _timer.Elapsed += ElapsedEventHandler;
            _dateTime = new DateTime(2022, 06, 01);
            _endTime = DateTime.Now.Date;
            _timer.Start();
        }

        public static async void ElapsedEventHandler(object? sender, ElapsedEventArgs e)
        {
            await Work(_dateTime);
            _dateTime = _dateTime.AddDays(1);
            if (_dateTime > _endTime)
            {
                _timer.Stop();
            }
        }
    }
}
