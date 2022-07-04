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
    public class StockNewsSchedule
    {
        public static IServiceProvider _serviceProvider { get; set; }

        public static Timer _timer { get; set; }

        public static DateTime _endTime { get; set; }

        public static DateTime _dateTime { get; set; }

        public StockNewsSchedule(IServiceProvider serviceProvider)
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

            if (date.Hour >= 8 && date.Hour <= 22)
            {
                if (date.Hour >= 9 && date.Hour < 14 && date.Minute % 10 == 0)
                {
                    isDo = true;
                }
                else if (date.Minute % 20 == 0)
                {
                    isDo = true;
                }
            }

            return isDo;
        }

        public static async Task Work(DateTime date, bool isMessage = false)
        {
            date = date.Date;
            var bo = await StockNewsScrubbing.WorkFromDate(date);
            var daos = await SaveData(bo, date);
            if (daos.IsNotEmpty() && isMessage)
            {
                var message = Message(daos);
                foreach (var msg in message)
                {
                    await MessageHelper.SendMessageAsync(ChannelEnum.StockNews, msg);
                }
            }
        }

        public static async Task<List<StockNewsDao>> SaveData(List<StockNewsBO> stockNewsBO, DateTime date)
        {
            using var scope = _serviceProvider.CreateScope();
            var stockNewsRepository = scope.ServiceProvider.GetService<StockNewsRepository>();
            var codes = await scope.ServiceProvider.GetService<StockCodeRepository>().GetExsitID();
            var exsitIDs = await stockNewsRepository.GetExsitID(date);

            var dao = stockNewsBO.Select(r => r.BuildDao()).ToList();

            dao = dao.Where(r => !exsitIDs.Contains(r.ID) && codes.Contains(r.StockCode)).ToList();
            await stockNewsRepository.Insert(dao);

            return dao;
        }

        public static List<string> Message(List<StockNewsDao> data)
        {
            var msg = data.Where(r => r.Type == StockNewsEnum.SelfAssessment).Select(r => $"{r.StockName}({r.StockCode})\r\n標題:{r.Titile}\r\n\r\n內容:{r.Content}").ToList();

            return msg;
        }

        public static void UpdateHistory()
        {
            _timer = new Timer(10 * 1000);
            _timer.Elapsed += ElapsedEventHandler;
            _dateTime = new DateTime(2022, 6, 6);
            _endTime = DateTime.Now.Date;
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
            _dateTime = _dateTime.AddDays(1);
            if (_dateTime > _endTime)
            {
                _timer.Stop();
            }
        }
    }
}
