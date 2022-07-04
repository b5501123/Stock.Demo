using HtmlAgilityPack;
using Stock.Common.Helpers;
using Stock.Model.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Service.Scrubbing
{
    public static class StockCodeScrubbing
    {
        public static async Task<List<StockCodeBO>> WorkFromDate(DateTime date)
        {
            var siiUrl = $"https://isin.twse.com.tw/isin/class_main.jsp?owncode=&stockname=&isincode=&market=1&issuetype=1&industry_code=&Page=1&chklike=Y";
            var otcUrl = $"https://isin.twse.com.tw/isin/class_main.jsp?owncode=&stockname=&isincode=&market=2&issuetype=4&industry_code=&Page=1&chklike=Y";
            List<StockCodeBO> list = new List<StockCodeBO>();
            var sii = await GetHtml(siiUrl);
            var otc = await GetHtml(otcUrl);

            list.AddRange(Analytics(sii,date));
            list.AddRange(Analytics(otc, date));
            return list;
        }

        public static async Task<string> GetHtml(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            string html = "";
            using (HttpClient httpClient = new HttpClient())
            {
                var res = await httpClient.SendAsync(request);
                var bytes = await res.Content.ReadAsByteArrayAsync();
                html = EncodeHelper.Big5ToString(bytes);

            }
            return html;
        }

        public static List<StockCodeBO> Analytics(string html, DateTime date)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode;
            var result = node.SelectNodes("//table[contains(@class, 'h4')]/tr").Select(r =>
            {
                var table = r.SelectNodes("td").Select(t => t.InnerText).ToList();
                var stock = table[5] != "股票" ? null : new StockCodeBO
                {
                    StockCode = table[2],
                    StockName = table[3],
                    DataTime = DateTime.Parse(table[7]),
                    CreateTime = DateTime.Now,
                };
                return stock;
            }).Where(w => w != null).ToList();

            return result;
        }
    }
}
