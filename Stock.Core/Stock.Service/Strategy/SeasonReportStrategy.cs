using Stock.Model.Dao;

namespace Stock.Strategy.Strategy
{
    public class SeasonReportStrategy
    {
        public static int EpsRank(List<SeasonReportDao> data)
        {
            var first = data.OrderByDescending(d => d.DataTime).FirstOrDefault().EPS;
            var sort = data.OrderByDescending(d => d.EPS).Select(r => r.EPS).ToList();
            var index = sort.IndexOf(first);

            return index;
        }
        public static bool IsTurnLossIntoProfit(List<SeasonReportDao> data)
        {
            var result = false;
            if (data.Count > 5)
            {
                var temp = data.OrderByDescending(d => d.DataTime);
                var first = temp.FirstOrDefault();
                result = temp.Skip(1).Take(4).Any(r => r.EPS <= 0) && first.EPS > 0;
            }
            return result;
        }
        public static bool IsHighest(List<SeasonReportDao> data)
            => EpsRank(data) == 0;
        public static bool IsSecond(List<SeasonReportDao> data)
           => EpsRank(data) == 1;


    }
}
