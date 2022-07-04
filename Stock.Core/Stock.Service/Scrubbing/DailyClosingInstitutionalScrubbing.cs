using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stock.Common.Extension;
using Stock.Common.Helpers;
using Stock.Model.BO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Service.Scrubbing
{
    public static class DailyClosingInstitutionalScrubbing
    {
        public static async Task<List<DailyClosingInstitutionalBO>> WorkFromDate(DateTime date)
        {
            var siiUrl = $"https://www.twse.com.tw/fund/T86?response=json&date={date.ToString("yyyyMMdd")}&selectType=ALLBUT0999";
            var otcUrl = $"https://www.tpex.org.tw/web/stock/3insti/daily_trade/3itrade_hedge_result.php?l=zh-tw&se=EW&t=D&d={date.Year - 1911}/{date.ToString("MM/dd")}";
            List<DailyClosingInstitutionalBO> list = new List<DailyClosingInstitutionalBO>();
            var sii = await GetHtml(siiUrl);
            var otc = await GetHtml(otcUrl);

            list.AddRange(SIIAnalytics(sii,date));
            list.AddRange(OTCAnalytics(otc, date));
            return list;
        }

        public static async Task<string> GetHtml(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            string html = "";
            using (HttpClient httpClient = new HttpClient())
            {
                var res = await httpClient.SendAsync(request);
                html = await res.Content.ReadAsStringAsync();

            }
            return html;
        }

        public static List<DailyClosingInstitutionalBO> SIIAnalytics(string html, DateTime date)
        {
            var obj = JObject.Parse(html).GetValue("data")?.ToString();

            if (obj.IsEmptyOrNull())
            {
                return new List<DailyClosingInstitutionalBO>();
            }

            var data = JsonConvert.DeserializeObject<List<List<string>>>(obj);

            var closeMarket = data.Select(field =>
            {
                int intTemp = 0;
                var doc = new DailyClosingInstitutionalBO
                {
                    StockCode = field[0],
                    StockName = field[1],
                    ForeignOver = int.TryParse(field[4], NumberStyles.Number, null, out intTemp) ? intTemp / 1000 : 0,
                    InvestmentOver = int.TryParse(field[10], NumberStyles.Number, null, out intTemp) ? intTemp / 1000 : 0,
                    DealerOver = int.TryParse(field[11], NumberStyles.Number, null, out intTemp) ? intTemp / 1000 : 0,
                    TreeInstitutionalOver = int.TryParse(field[18], NumberStyles.Number, null, out intTemp) ? intTemp / 1000 : 0,
                    DataTime = date,
                    CreateTime = DateTime.UtcNow.AddHours(8)
                };

                return doc;
            }).ToList();

            return closeMarket;
        }

        public static List<DailyClosingInstitutionalBO> OTCAnalytics(string html, DateTime date)
        {
            var obj = JObject.Parse(html).GetValue("aaData")?.ToString();
            if (obj.IsEmptyOrNull())
            {
                return new List<DailyClosingInstitutionalBO>();
            }

            var data = JsonConvert.DeserializeObject<List<List<string>>>(obj);

            var closeMarket = data.Select(field =>
            {
                int intTemp = 0;
                var doc = new DailyClosingInstitutionalBO
                {
                    StockCode = field[0],
                    StockName = field[1],
                    ForeignOver = int.TryParse(field[4], NumberStyles.Number, null, out intTemp) ? intTemp / 1000 : 0,
                    InvestmentOver = int.TryParse(field[13], NumberStyles.Number, null, out intTemp) ? intTemp / 1000 : 0,
                    DealerOver = int.TryParse(field[22], NumberStyles.Number, null, out intTemp) ? intTemp / 1000 : 0,
                    TreeInstitutionalOver = int.TryParse(field[23], NumberStyles.Number, null, out intTemp) ? intTemp / 1000 : 0,
                    DataTime = date,
                    CreateTime = DateTime.UtcNow.AddHours(8)
                };
                return doc;
            }).ToList();

            return closeMarket;
        }
    }
}
