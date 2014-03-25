using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TD.Utils.ExtensionMethods;

namespace TD.Utils.ExtensionMethods
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void FutureMonthly()
        {
            DateTime date = DateTime.Parse("1/1/2020");
            DateTime time = date.AddHours(10);

            date = date.CalculateNextOccurrence(time, "m3");

            Assert.AreEqual<DateTime>(DateTime.Parse("1/3/2020 10:00:00 AM"), date);
        }

        [TestMethod]
        public void FutureMonthlyDayOnThePast()
        {
            DateTime date = DateTime.Parse("1/5/2020");
            DateTime time = date.AddHours(10);

            date = date.CalculateNextOccurrence(time, "m3");

            Assert.AreEqual<DateTime>(DateTime.Parse("2/3/2020 10:00:00 AM"), date);
        }

        [TestMethod]
        public void PastMonthly()
        {
            DateTime date = DateTime.Parse("1/1/2010");
            DateTime time = date.AddHours(23).AddMinutes(30);

            date = date.CalculateNextOccurrence(time, "m30");

            Assert.AreEqual<DateTime>(DateTime.Parse("6/30/2013 11:30:00 PM"), date);
        }

        [TestMethod]
        public void PastMonthlyDayOnThePast()
        {
            DateTime date = DateTime.Parse("1/5/2010");
            DateTime time = date.AddHours(1);

            date = date.CalculateNextOccurrence(time, "m31");

            Assert.AreEqual<DateTime>(DateTime.Parse("6/30/2013 1:00:00 AM"), date);
        }

        [TestMethod]
        public void FutureDaily()
        {
            DateTime date = DateTime.Parse("1/1/2020");
            DateTime time = date.AddHours(10);

            date = date.CalculateNextOccurrence(time, "d");

            Assert.AreEqual<DateTime>(DateTime.Parse("1/2/2020 10:00:00 AM"), date);
        }

        [TestMethod]
        public void PastDaily()
        {
            DateTime date = DateTime.Parse("1/1/2010");
            DateTime time = date.AddHours(10);

            date = date.CalculateNextOccurrence(time, "d");

            Assert.AreEqual<DateTime>(DateTime.Parse("5/31/2013 10:00:00 AM"), date);
        }

        [TestMethod]
        public void PastDaily2()
        {
            DateTime date = DateTime.Parse("1/1/2010");
            DateTime time = date.AddHours(1);

            date = date.CalculateNextOccurrence(time, "dw");

            Assert.AreEqual<DateTime>(DateTime.Parse("6/3/2013 1:00:00 AM"), date);
        }

        [TestMethod]
        public void FutureDailySaturday()
        {
            DateTime date = DateTime.Parse("1/4/2014");
            DateTime time = date.AddHours(10);

            date = date.CalculateNextOccurrence(time, "dw");

            Assert.AreEqual<DateTime>(DateTime.Parse("1/6/2014 10:00:00 AM"), date);
        }

        [TestMethod]
        public void FutureDailySunday()
        {
            DateTime date = DateTime.Parse("1/5/2014");
            DateTime time = date.AddHours(10);

            date = date.CalculateNextOccurrence(time, "dw");

            Assert.AreEqual<DateTime>(DateTime.Parse("1/6/2014 10:00:00 AM"), date);
        }

        [TestMethod]
        public void FutureWeeklySameDay()
        {
            DateTime date = DateTime.Parse("1/5/2014");
            DateTime time = date.AddHours(10);

            date = date.CalculateNextOccurrence(time, "W1");

            Assert.AreEqual<DateTime>(DateTime.Parse("1/5/2014 10:00:00 AM"), date);
        }

        [TestMethod]
        public void FutureWeeklySameDay2()
        {
            DateTime date = DateTime.Parse("1/4/2014");
            DateTime time = date.AddHours(23).AddMinutes(50);

            date = date.CalculateNextOccurrence(time, "W67");

            Assert.AreEqual<DateTime>(DateTime.Parse("1/4/2014 23:50:00 PM"), date);
        }

        [TestMethod]
        public void FutureWeeklyFutureDay()
        {
            DateTime date = DateTime.Parse("1/5/2014");
            DateTime time = date.AddMinutes(5);

            date = date.CalculateNextOccurrence(time, "W7");

            Assert.AreEqual<DateTime>(DateTime.Parse("1/11/2014 12:05:00 AM"), date);
        }

        [TestMethod]
        public void FutureWeeklyDayOnThePast()
        {
            DateTime date = DateTime.Parse("1/3/2014");
            DateTime time = date.AddMinutes(5);

            date = date.CalculateNextOccurrence(time, "W4");

            Assert.AreEqual<DateTime>(DateTime.Parse("1/8/2014 12:05:00 AM"), date);
        }

        [TestMethod]
        public void PastWeekly()
        {
            DateTime date = DateTime.Parse("1/4/2010");
            DateTime time = date.AddHours(23).AddMinutes(50);

            date = date.CalculateNextOccurrence(time, "W67");

            Assert.AreEqual<DateTime>(DateTime.Parse("5/31/2013 11:50:00 PM"), date);
        }

        [TestMethod]
        public void PastWeekly2()
        {
            DateTime date = DateTime.Parse("1/4/2010");
            DateTime time = date.AddHours(23).AddMinutes(50);

            date = date.CalculateNextOccurrence(time, "W7");

            Assert.AreEqual<DateTime>(DateTime.Parse("6/1/2013 11:50:00 PM"), date);
        }

        [TestMethod]
        public void PastWeeklyPastHour()
        {
            DateTime date = DateTime.Parse("1/4/2010");
            DateTime time = date.AddMinutes(5);

            date = date.CalculateNextOccurrence(time, "W67");

            Assert.AreEqual<DateTime>(DateTime.Parse("6/1/2013 12:05:00 AM"), date);
        }

        [TestMethod]
        public void PastWeeklyPastHour2()
        {
            DateTime date = DateTime.Parse("1/4/2010");
            DateTime time = date.AddMinutes(5);

            date = date.CalculateNextOccurrence(time, "W6");

            Assert.AreEqual<DateTime>(DateTime.Parse("6/7/2013 12:05:00 AM"), date);
        }
    }
}
