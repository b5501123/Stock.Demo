using System;

namespace Stock.Common.Extension
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// Get first day of month
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(this DateTime date)
        {
            return GetDayOfMonth(date, 1);
        }

        public static DateTime GetFirstDayOfYear(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        /// <summary>
        /// Get day fo month
        /// </summary>
        /// <param name="date"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime GetDayOfMonth(this DateTime date, int day)
        {
            return new DateTime(date.Year, date.Month, day);
        }


        /// <summary>
        /// Get last day of month
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(this DateTime date)
        {
            var resultTime = new DateTime(date.Year, date.Month,
                DateTime.DaysInMonth(date.Year, date.Month));
            return resultTime;
        }

        //public static DateTime GetSeasonTime(int year, SeasonEnum season, DateTimeKind kind = DateTimeKind.Utc)
        //{
        //    DateTime result;

        //    switch (season)
        //    {
        //        case SeasonEnum.Q1:
        //            result = new DateTime(year, 3, 31, 0, 0, 0, kind);
        //            break;
        //        case SeasonEnum.Q2:
        //            result = new DateTime(year, 6, 30, 0, 0, 0, kind);
        //            break;
        //        case SeasonEnum.Q3:
        //            result = new DateTime(year, 9, 30, 0, 0, 0, kind);
        //            break;
        //        case SeasonEnum.Q4:
        //        default:
        //            result = new DateTime(year, 12, 31, 0, 0, 0, kind);
        //            break;
        //    }

        //    return result;
        //}

        /// <summary>
        /// Get two date between days
        /// </summary>
        /// <param name="date"></param>
        /// <param name="compareDate"></param>
        /// <returns></returns>
        public static int GetDiffDays(this DateTime date, DateTime compareDate)
        {
            return (int)(date - compareDate).TotalDays;
        }

        /// <summary>
        /// Get last day of week
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfWeek(this DateTime date, DayOfWeek lastDayOfWeek = DayOfWeek.Saturday)
        {
            var daysAhead = (int)lastDayOfWeek - (int)date.DayOfWeek;
            return date.Date.AddDays(daysAhead < 0 ? daysAhead + 7 : daysAhead);
        }


        /// <summary>
        /// Is date between startDate and endDate (contains)
        /// </summary>
        /// <param name="date"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static bool IsBetweenAndContains(this DateTime date, DateTime startDate, DateTime endDate)
        {
            return date >= startDate && date <= endDate;
        }

        /// <summary>
        /// Is date between startDate and endDate
        /// </summary>
        /// <param name="date"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static bool IsBetween(this DateTime date, DateTime startDate, DateTime endDate)
        {
            return date > startDate && date < endDate;
        }

        /// <summary>
        /// Convert to Date Format String
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy/MM/dd");
        }

        /// <summary>
        /// Convert to Time Format String
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss");
        }

        /// <summary>
        /// Convert to Date Format String
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetDateString(this DateTime? dateTime)
        {
            return !dateTime.HasValue ? "" : dateTime.Value.ToString("yyyy/MM/dd");
        }

        /// <summary>
        /// Convert to Time Format String
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetTimeString(this DateTime? dateTime)
        {
            return !dateTime.HasValue ? "" : dateTime.Value.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 轉換日期為 ISO 8601 格式字串(精確到毫秒)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToISO8601(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                return (DateTime.SpecifyKind(dateTime, DateTimeKind.Local)).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFK");
            }
            else
            {
                return dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFK");
            }
        }

        public static DateTime Add8H(this DateTime dateTime)
        {
            return dateTime.AddHours(8);
        }
    }
}
