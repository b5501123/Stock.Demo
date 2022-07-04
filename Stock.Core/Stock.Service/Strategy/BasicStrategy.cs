using Stock.Model.Dao;

namespace Stock.Service.Strategy
{
    public class BasicStrategy
    {
        public static int IncomeRank(List<MothIncomeDao> data)
        {
            var first = data.OrderByDescending(d => d.DataTime).FirstOrDefault().CurrentIncome;
            var sort = data.OrderByDescending(d => d.CurrentIncome).Select(r => r.CurrentIncome).ToList();
            var index = sort.IndexOf(first);

            return index;
        }
        public static bool YoYover3Month(List<MothIncomeDao> data, decimal over = 100)
        {
            var sort = data.OrderByDescending(d => d.DataTime).Take(3);

            return sort.All(r => r.YearAdd >= over);
        }
        public static bool MoMover3Month(List<MothIncomeDao> data, decimal over = 0)
        {
            var sort = data.OrderByDescending(d => d.DataTime).Take(3);

            return sort.All(r => r.MonthAdd >= over);
        }
        public static bool BothMonYearAdd(List<MothIncomeDao> data, int take = 3)
        {
            var sort = data.OrderByDescending(d => d.DataTime).Take(take);

            return sort.All(r => r.YearAdd > 0 && r.MonthAdd > 0);
        }

        public static int Over100Count(List<MothIncomeDao> data)
        {
            var sort = data.OrderByDescending(d => d.DataTime).Take(24);

            int count = 0;

            foreach (var item in sort)
            {
                if (item.YearAdd >= 100)
                    count++;
                else
                    return count;
            }

            return count;
        }
        public static bool IsHighest(List<MothIncomeDao> data)
            => IncomeRank(data) == 0;
        public static bool IsSecond(List<MothIncomeDao> data)
           => IncomeRank(data) == 1;


    }
}
