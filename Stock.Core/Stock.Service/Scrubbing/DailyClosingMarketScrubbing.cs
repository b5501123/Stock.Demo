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
    public static class DailyClosingMarketScrubbing
    {
        public static async Task<List<DailyClosingMarketBO>> WorkFromDate(DateTime date)
        {
            var siiUrl = $"https://www.twse.com.tw/exchangeReport/MI_INDEX?response=json&date={date.ToString("yyyyMMdd")}&type=ALLBUT0999";
            var otcUrl = $"https://www.tpex.org.tw/web/stock/aftertrading/otc_quotes_no1430/stk_wn1430_result.php?l=zh-tw&d={date.Year - 1911}/{date.ToString("MM/dd")}&se=EW";
            List<DailyClosingMarketBO> list = new List<DailyClosingMarketBO>();
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

        public static List<DailyClosingMarketBO> SIIAnalytics(string html, DateTime date)
        {
            var obj = JObject.Parse(html).GetValue("data9")?.ToString();
            if (obj.IsEmptyOrNull())
            {
                return new List<DailyClosingMarketBO>();
            }
            var data = JsonConvert.DeserializeObject<List<List<string>>>(obj);
            var closeMarket = new List<DailyClosingMarketBO>();
            try
            {

                closeMarket = data.Select(field =>
                {
                    int intTemp = 0;
                    decimal decimalTemp = 0;
                    long longTemp = 0;
                    var doc = new DailyClosingMarketBO
                    {
                        StockCode = field[0],
                        StockName = field[1],
                        Volume = int.TryParse(field[2], NumberStyles.Number, null, out intTemp) ? intTemp / 1000 : 0,
                        Count = int.TryParse(field[3], NumberStyles.Number, null, out intTemp) ? intTemp : 0,
                        DealMoney = long.TryParse(field[4], NumberStyles.Number, null, out longTemp) ? longTemp : 0,
                        Open = decimal.TryParse(field[5], NumberStyles.Number, null, out decimalTemp) ? decimal.Round(decimalTemp, 2) : 0,
                        High = decimal.TryParse(field[6], NumberStyles.Number, null, out decimalTemp) ? decimal.Round(decimalTemp, 2) : 0,
                        Low = decimal.TryParse(field[7], NumberStyles.Number, null, out decimalTemp) ? decimal.Round(decimalTemp, 2) : 0,
                        Close = decimal.TryParse(field[8], NumberStyles.Number, null, out decimalTemp) ? decimal.Round(decimalTemp, 2) : 0,
                        UpDown = decimal.TryParse(field[10], NumberStyles.Number, null, out decimalTemp) ? decimal.Round(decimalTemp, 2) : 0,
                        DataTime = date,
                        CreateTime = DateTime.UtcNow.AddHours(8)

                    };
                    doc.UpDown = field[10].Contains("green") ? doc.UpDown * -1 : doc.UpDown;
                    var lastClose = doc.Close - doc.UpDown;
                    doc.Increase = lastClose != 0 ? Math.Round(doc.UpDown * 100 / lastClose, 2, MidpointRounding.AwayFromZero) : 0;
                    doc.Amplitude = lastClose != 0 ? Math.Round((doc.High - doc.Low) * 100 / lastClose, 2, MidpointRounding.AwayFromZero) : 0;
                    doc.AverageSheet = doc.Count != 0 ? Math.Round(doc.Volume / (decimal)doc.Count, 1, MidpointRounding.AwayFromZero) : 0;

                    return doc;
                }).ToList();
            }
            catch{ }

            return closeMarket;
        }

        public static List<DailyClosingMarketBO> OTCAnalytics(string html, DateTime date)
        {
            var obj = JObject.Parse(html).GetValue("aaData")?.ToString();
            if (obj.IsEmptyOrNull())
            {
                return new List<DailyClosingMarketBO>();
            }

            var data = JsonConvert.DeserializeObject<List<List<string>>>(obj);
            var closeMarket = new List<DailyClosingMarketBO>();
            try 
            {
                closeMarket = data.Select(field =>
                {
                    int intTemp = 0;
                    decimal decimalTemp = 0;
                    var doc = new DailyClosingMarketBO
                    {
                        StockCode = field[0],
                        StockName = field[1],
                        Close = decimal.TryParse(field[2], NumberStyles.Number, null, out decimalTemp) ? decimalTemp : 0,
                        UpDown = decimal.TryParse(field[3], NumberStyles.Number, null, out decimalTemp) ? decimalTemp : 0,
                        Open = decimal.TryParse(field[4], NumberStyles.Number, null, out decimalTemp) ? decimalTemp : 0,
                        High = decimal.TryParse(field[5], NumberStyles.Number, null, out decimalTemp) ? decimalTemp : 0,
                        Low = decimal.TryParse(field[6], NumberStyles.Number, null, out decimalTemp) ? decimalTemp : 0,
                        Volume = int.TryParse(field[7], NumberStyles.Number, null, out intTemp) ? intTemp / 1000 : 0,
                        DealMoney = int.TryParse(field[8], NumberStyles.Number, null, out intTemp) ? intTemp : 0,
                        Count = int.TryParse(field[9], NumberStyles.Number, null, out intTemp) ? intTemp : 0,
                        DataTime = date,
                        CreateTime = DateTime.UtcNow.AddHours(8)

                    };
                    var lastClose = doc.Close - doc.UpDown;
                    doc.Increase = lastClose != 0 ? Math.Round(doc.UpDown * 100 / lastClose, 2, MidpointRounding.AwayFromZero) : 0;
                    doc.Amplitude = lastClose != 0 ? Math.Round((doc.High - doc.Low) * 100 / lastClose, 2, MidpointRounding.AwayFromZero) : 0;
                    doc.AverageSheet = doc.Count != 0 ? Math.Round(doc.Volume / (decimal)doc.Count, 1, MidpointRounding.AwayFromZero) : 0;
                    return doc;
                }).ToList();
            }
            catch { }

            return closeMarket;
        }
    }
}
