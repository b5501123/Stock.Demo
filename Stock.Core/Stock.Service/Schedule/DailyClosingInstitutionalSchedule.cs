using Microsoft.Extensions.DependencyInjection;
using Stock.Model.BO;
using Stock.Model.Dao;
using Stock.Repository;
using Stock.Service.Scrubbing;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Stock.Service.Schedule
{
    public class DailyClosingInstitutionalSchedule
    {
        public static IServiceProvider _serviceProvider { get; set; }

        public static Timer _timer { get; set; }

        public static DateTime _endTime { get; set; }

        public static DateTime _dateTime { get; set; }

        public DailyClosingInstitutionalSchedule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static async void JobInvoke(object? sender, ElapsedEventArgs e)
        {
            if(IsDo())
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
            var bo = await DailyClosingInstitutionalScrubbing.WorkFromDate(date);
            await SaveData(bo, date);
        }

        public static async Task<List<DailyClosingInstitutionalDao>> SaveData(List<DailyClosingInstitutionalBO> dailyClosingInstitutionalBO, DateTime date)
        {
            using var scope = _serviceProvider.CreateScope();
            var dailyClosingInstitutionalRepository = scope.ServiceProvider.GetService<DailyClosingInstitutionalRepository>();
            var codes = await scope.ServiceProvider.GetService<StockCodeRepository>().GetExsitID();
            var exsitIDs = await dailyClosingInstitutionalRepository.GetExsitID(date);

            var dao = dailyClosingInstitutionalBO.Select(r => r.BuildDao()).ToList();
            dao = dao.Where(r => !exsitIDs.Contains(r.ID) && codes.Contains(r.StockCode)).ToList();
            await dailyClosingInstitutionalRepository.Insert(dao);

            return dao;
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
