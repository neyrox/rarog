using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class TableTest
    {
        private Table table;
        private readonly List<string> values1 = new List<string> { "1", "2", "3" };
        private readonly List<string> values2 = new List<string> { "4", "5", "6" };

        private readonly List<string> col1 = new List<string> { "1", "4" };
        private readonly List<string> col2 = new List<string> { "2", "5" };
        private readonly List<string> col3 = new List<string> { "3", "6" };

        [TestInitialize]
        public void Setup()
        {
            table = new Table();
            table.AddColumn("c1", "int", 0);
            table.AddColumn("c2", "int", 0);
            table.AddColumn("c3", "int", 0);
            var columnNames = new List<string> { "c1", "c2", "c3" };
            table.Insert(columnNames, values1);
            table.Insert(columnNames, values2);
        }

        [TestMethod]
        public void SelectReturnsAll()
        {
            var result = table.Select(new List<string> { "*" }, null);
            Assert.AreEqual(3, result.Count);
            CollectionAssert.AreEqual(col1, result[0].All());
            CollectionAssert.AreEqual(col2, result[1].All());
            CollectionAssert.AreEqual(col3, result[2].All());
        }

        [TestMethod]
        public void CorrespondingRowSelected()
        {
            var condition = new ConditionNode("c1", "=", "4");

            var result = table.Select(new List<string> { "*" }, condition);
            Assert.AreEqual(3, result.Count);
            CollectionAssert.AreEqual(ToList("4"), result[0].All());
            CollectionAssert.AreEqual(ToList("5"), result[1].All());
            CollectionAssert.AreEqual(ToList("6"), result[2].All());
        }

        [TestMethod]
        public void CorrespondingRowUpdated()
        {
            var condition = new ConditionNode("c1", "=", "4");

            table.Update(new List<string> { "c2" }, new List<string> { "10" }, condition);
            var values2Updated = new List<string> { "4", "10", "6" };

            var result = table.Select(new List<string> { "*" }, null);
            Assert.AreEqual(3, result.Count);
            CollectionAssert.AreEqual(col1, result[0].All());
            CollectionAssert.AreEqual(new List<string> { "2", "10" }, result[1].All());
            CollectionAssert.AreEqual(col3, result[2].All());
        }

        private List<string> ToList(string item)
        {
            return new List<string> { item };
        }
    }
}
