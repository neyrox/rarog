using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class TableTest
    {
        [TestMethod]
        public void TableReturnsAll()
        {
            var table = new Table();
            table.AddColumn("c1", "int", 0);
            table.AddColumn("c2", "int", 0);
            table.AddColumn("c3", "int", 0);
            var columnNames = new List<string> { "c1", "c2", "c3" };
            var values1 = new List<string> { "123", "234", "345" };
            var values2 = new List<string> { "567", "678", "789" };
            table.Insert(columnNames, values1);
            table.Insert(columnNames, values2);
            var result = table.Select(new List<string> { "*" });
            Assert.AreEqual(2, result.Count);
            CollectionAssert.AreEqual(values1, result[0]);
            CollectionAssert.AreEqual(values2, result[1]);
        }
    }
}
