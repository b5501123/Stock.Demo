using Stock.Common.Enums;
using Stock.Common.Extension;
using Stock.Model.Dao;

namespace Stock.Model.BO
{
    public class StockNewsBO : BaseStockBO
    {
        public string Key { get; set; }
        public StockNewsEnum Type { get; set; }
        public string Titile { get; set; }
        public string Content { get; set; }
        public StockNewsEnum GetType(string title)
        {
            var type = StockNewsEnum.Other;
            if (title.IsNotEmpty())
            {
                type = IsSelfAssessment(title) ? StockNewsEnum.SelfAssessment : type;
            }

            return type;
        }
        public bool IsSelfAssessment(string title)
        {
            return title.Contains("自結") && title.Contains("月") && (title.Contains("損益") || title.Contains("盈餘"));
        }

        public StockNewsDao BuildDao()
        {
            var dao = this.Reflect<StockNewsDao>();
            dao.ID = this.DataTime.ToString("yyyyMMddHHmmss") + dao.StockCode;
            if (dao.Titile.Length > 200)
            {
                dao.Titile = dao.Titile.Substring(0, 200);
            }
            if (dao.Content.Length > 5000)
            {
                dao.Content = dao.Content.Substring(0, 5000);
            }
            return dao;
        }
    }
}
