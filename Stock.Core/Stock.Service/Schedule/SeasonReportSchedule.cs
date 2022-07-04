using Microsoft.Extensions.DependencyInjection;
using Stock.Model.BO;
using Stock.Repository;
using Stock.Service.Scrubbing;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Stock.Service.Schedule
{
    public class SeasonReportSchedule
    {
        public static IServiceProvider _serviceProvider { get; set; }

        public static Timer _timer { get; set; }

        public static DateTime _endTime { get; set; }

        public static DateTime _dateTime { get; set; }

        public SeasonReportSchedule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static async void JobInvoke(object? sender, ElapsedEventArgs e)
        {
            if (IsDo())
            {
                var time = GetPreSeasion(DateTime.Now.Date);
                await Work(time);
            }
        }

        public static bool IsDo()
        {
            var time = DateTime.Now;
            var isDo = false;
            if (time.Month != 12 && time.Month != 9 && time.Month != 6 && time.Month != 3)
            {

                if (time.Hour >= 8 && time.Hour < 23)
                {
                    if (time.Hour >= 9 && time.Hour <= 14 && time.Minute % 5 == 0)
                    {
                        isDo = true;
                    }
                    else if (time.Minute % 20 == 0)
                        isDo = true;
                }
            }

            return isDo;
        }
        public static async Task Work(DateTime date)
        {
            date = date.Date;
            var bo = await SeasonReportScrubbing.WorkFromDate(date);
            await SaveData(bo, date);
        }

        public static async Task SaveData(List<SeasonReportBO> seasonReportBO, DateTime date)
        {
            using var scope = _serviceProvider.CreateScope();
            var seasonReportRepository = scope.ServiceProvider.GetService<SeasonReportRepository>();
            var codes = await scope.ServiceProvider.GetService<StockCodeRepository>().GetExsitID();
            var exsitIDs = await seasonReportRepository.GetExsitID(date);

            var dao = seasonReportBO.Select(r => r.BuildDao()).ToList();

            dao = dao.Where(r => !exsitIDs.Contains(r.ID) && codes.Contains(r.StockCode)).ToList();
            await seasonReportRepository.Insert(dao);
        }

        public static void UpdateHistory()
        {
            _timer = new Timer(10 * 1000);
            _timer.Elapsed += ElapsedEventHandler;
            _dateTime = new DateTime(2018, 3, 1);
            _endTime = DateTime.Now;
            _timer.Start();
        }

        public static DateTime GetPreSeasion(DateTime time)
        {
            if (time.Month >= 1 && time.Month <= 3)
                return new DateTime(time.Year - 1, 12, 1);
            else if (time.Month >= 4 && time.Month <= 6)
                return new DateTime(time.Year, 3, 1);
            else if (time.Month >= 7 && time.Month <= 9)
                return new DateTime(time.Year, 6, 1);
            else
                return new DateTime(time.Year, 9, 1);
        }

        public static async void ElapsedEventHandler(object? sender, ElapsedEventArgs e)
        {
            await Work(_dateTime);
            _dateTime = _dateTime.AddMonths(3);
            if (_dateTime > _endTime)
            {
                _timer.Stop();
            }
        }
    }
}
