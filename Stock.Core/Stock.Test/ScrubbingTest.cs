using NUnit.Framework;
using Stock.Common.Enums;
using Stock.Common.Helpers;
using Stock.Service.Scrubbing;
using System;
using System.Collections.Generic;
using Telegram.Bot;

namespace Stock.Test
{
    public class ScrubbingTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void StockCodeScrubbingTest()
        {
            var list = StockCodeScrubbing.WorkFromDate(DateTime.Now).Result;
            Assert.Pass();
        }

        [Test]
        public void DailyClosingMarketScrubbingTest()
        {
            var list = DailyClosingMarketScrubbing.WorkFromDate(DateTime.Now).Result;
            Assert.Pass();
        }

        [Test]
        public void MonthincomeScrubbingTest()
        {
            var list = MonthincomeScrubbing.WorkFromDate(DateTime.Now.AddMonths(-2)).Result;
            Assert.Pass();
        }

        
        [Test]
        public void StockNewsScrubbingTest()
        {
            var list = StockNewsScrubbing.WorkFromDate(DateTime.Now).Result;
            Assert.Pass();
        }

        [Test]
        public void DailyClosingInstitutionalScrubbingTest()
        {
            var list = SeasonReportScrubbing.WorkFromDate(new DateTime(2022,03,01)).Result;
            Assert.Pass();
        }

        [Test]
        public void TelegramBotTest()
        {
            ConfigHelper.TelegramBotApi = "5515984528:AAE6HsckiyYilWP4iGSb9b8GCyyUhDaLjLk";

            var list = new List<long> { ChannelEnum.EPS,ChannelEnum.StockNews };

            MessageHelper.SendMessageAsync(list, "test").Wait();
        }

    }
}