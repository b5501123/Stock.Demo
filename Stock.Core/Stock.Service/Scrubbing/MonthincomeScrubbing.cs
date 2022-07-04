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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Stock.Service.Scrubbing
{
    public static class MonthincomeScrubbing
    {
        public static async Task<List<MonthIncomeBO>> WorkFromDate(DateTime date)
        {
            var siiUrl = $"https://mops.twse.com.tw/nas/t21/sii/t21sc03_{date.Year - 1911}_{date.Month}_0.html";
            var otcUrl = $"https://mops.twse.com.tw/nas/t21/otc/t21sc03_{date.Year - 1911}_{date.Month}_0.html";
            List<MonthIncomeBO> list = new List<MonthIncomeBO>();
            var sii = await GetHtml(siiUrl);
            var otc = await GetHtml(otcUrl);

            list.AddRange(Analytics(sii, date));
            list.AddRange(Analytics(otc, date));
            var temp = list.OrderBy(r => r.YearAdd).ToList();
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

        public static List<MonthIncomeBO> Analytics(string html, DateTime date)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var incomeReport = new List<MonthIncomeBO>();
            var node = doc.DocumentNode;
            try
            {

                var result1 = node.SelectNodes("/html/body/center/center/table/tr/td/table").Nodes().Select(r =>
                {
                    var table = r.SelectNodes("td/table");
                    if (table.IsNotEmpty())
                    {
                        var t = table.Nodes().ToList();
                        table.Nodes().ToList().Select(f =>
                        {
                            var field = f.SelectNodes("td");
                            if (field.IsNotEmpty() && field.Count == 11)
                            {
                                var strs = field.Nodes().Select(r => Regex.Replace(r.InnerText, @"s+", "").Trim()).ToList();
                                incomeReport.Add(ToDocument(strs, date));
                            }
                            return true;
                        }).ToList();
                    }
                    return true;
                }).ToList();
            }
            catch { }
            return incomeReport;
        }

        public static MonthIncomeBO ToDocument(List<string> field, DateTime dataTime)
        {
            long longTemp = 0;
            decimal decimalTemp = 0;
            MonthIncomeBO dao = new MonthIncomeBO
            {
                StockCode = field[0],
                StockName = field[1],
                CurrentIncome = long.TryParse(field[2], NumberStyles.Number, null, out longTemp) ? longTemp : 0,
                LastIncome = long.TryParse(field[3], NumberStyles.Number, null, out longTemp) ? longTemp : 0,
                LY_CurrentIncome = long.TryParse(field[4], NumberStyles.Number, null, out longTemp) ? longTemp : 0,
                MonthAdd = decimal.TryParse(field[5], out decimalTemp) ? decimal.Round(decimalTemp, 2) : 0,
                YearAdd = decimal.TryParse(field[6], out decimalTemp) ? decimal.Round(decimalTemp, 2) : 0,
                CumulativeIncome = long.TryParse(field[7], NumberStyles.Number, null, out longTemp) ? longTemp : 0,
                LY_CumulativeIncome = long.TryParse(field[8], NumberStyles.Number, null, out longTemp) ? longTemp : 0,
                CumulativeAdd = decimal.TryParse(field[9], out decimalTemp) ? decimal.Round(decimalTemp, 2) : 0,
                Remark = field[10],
                DataTime = dataTime,
                CreateTime = DateTime.Now,
            };

            return dao;
        }
    }
}
