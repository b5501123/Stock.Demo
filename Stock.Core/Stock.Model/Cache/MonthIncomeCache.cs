
namespace Stock.Model.Cache
{
    public class MonthIncomeCache
    {
        public string StockCode { get; set; }
        public string StockName { get; set; }
        public decimal YoY { get; set; }
        public decimal MoM { get; set; }
        public decimal AllYoY { get; set; }
        public string Remark { get; set; }
        public int YoYOver100Count { get; set; }
        public bool IsHighest { get; set; }
        public bool IsSecond { get; set; }
        public bool Is3MonthYoY100 => YoYOver100Count >= 3;
        public bool IsMoMover3Month { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime DataTime { get; set; }
    }
}
