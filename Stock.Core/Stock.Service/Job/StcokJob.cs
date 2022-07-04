using Stock.Service.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Stock.Service.Job
{
    public class StcokJob
    {
        public StcokJob()
        {
            JobTimer.Elapsed += DailyClosingInstitutionalSchedule.JobInvoke;
            JobTimer.Elapsed += DailyClosingMarketSchedule.JobInvoke;
            JobTimer.Elapsed += MothIncomeSchedule.JobInvoke;
            JobTimer.Elapsed += SeasonReportSchedule.JobInvoke;
            JobTimer.Elapsed += StockNewsSchedule.JobInvoke;
            JobTimer.Elapsed += ClearMonery;
            //JobTimer.Elapsed += TimelyStockPriceSchedule.JobInvoke; 這需要配合卷商資料才能使用
        }

        public Timer JobTimer { get; set; } = new Timer(1000 * 60);

        public void ClearMonery(object? sender, ElapsedEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void Start()
        {
            JobTimer.Start();
        }

    }
}
