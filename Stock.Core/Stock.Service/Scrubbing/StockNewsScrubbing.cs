using HtmlAgilityPack;
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
    public static class StockNewsScrubbing
    {
        public static async Task<List<StockNewsBO>> WorkFromDate(DateTime date)
        {
            var url = $"https://mops.twse.com.tw/mops/web/ajax_t05st02";
            List<StockNewsBO> list = new List<StockNewsBO>();
            var html = await GetHtml(url, date);

            list.AddRange(Analytics(html,date));
            return list;
        }

        public static async Task<string> GetHtml(string url, DateTime dataTime)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            string html = "";
            var dic = new Dictionary<string, string>();
            dic.Add("firstin", "1");
            dic.Add("TYPEK", "1");
            dic.Add("year", (dataTime.Year - 1911).ToString());
            dic.Add("month", dataTime.Month.ToString());
            dic.Add("day", dataTime.Day.ToString());

            var form = new FormUrlEncodedContent(dic);


            using (HttpClient httpClient = new HttpClient())
            {
                html = await (await httpClient.PostAsync(url, form)).Content.ReadAsStringAsync();
            }
            return html;
        }

        public static List<StockNewsBO> Analytics(string html, DateTime date)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode;
            var result = node.SelectNodes("//tr[contains(@class, 'even') or contains(@class, 'odd')]")
                .Select(n => n.SelectNodes("td/input").Select(r => r.Attributes["value"].Value).ToList())
                .Where(r => r.Count == 10)
                .Select(r =>
                {
                    var data = new StockNewsBO
                    {
                        StockName = r[0],
                        StockCode = r[1],
                        Key = $"{r[2]}_{string.Format("{0:000000}", Convert.ToInt64(r[3]))}_{r[1]}",
                        Titile = r[4],
                        Content = r[8],
                        DataTime = DateTime.ParseExact(r[2] + string.Format("{0:000000}", Convert.ToInt64(r[3])), "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        CreateTime = DateTime.Now
                    };
                    data.Type = data.GetType(data.Titile);
                    return data;
                }).ToList();

            return result;


        }
    }
}
