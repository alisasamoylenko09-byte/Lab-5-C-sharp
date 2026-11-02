using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Lab5Variant5; // Назва вашого простору імен (namespace)

namespace TestProject505
{
    [TestClass]
    public class Test1
    {
        [TestMethod]
        public void TestCalculateTotalVisitors()
        {
            // Використовуємо клас Day замість ExhibitionDay
            List<Day> testList = new List<Day>
            {
                new Day("Виставка А", "Автор1", "Понеділок", 500, "Чудово"),
                new Day("Виставка Б", "Автор2", "Вівторок", 200, "Нормально")
            };

            int expectedTotal = 700;
            int actualTotal = 0;

            foreach (var item in testList)
            {
                actualTotal += item.VisitorsCount;
            }

            Assert.AreEqual(expectedTotal, actualTotal);
        }
    }
}