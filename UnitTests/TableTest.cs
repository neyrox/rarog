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
            var result = table.Select(new List<string> { "*" }, new List<ConditionNode>());
            Assert.AreEqual(2, result.Count);
            CollectionAssert.AreEqual(values1, result[0]);
            CollectionAssert.AreEqual(values2, result[1]);
        }

        [TestMethod]
        public void CorrespondingRowSelected()
        {
            var conditions = new List<ConditionNode>() { new ConditionNode("c1", "=", "4") };

            var result = table.Select(new List<string> { "*" }, conditions);
            Assert.AreEqual(1, result.Count);
            CollectionAssert.AreEqual(values2, result[0]);
        }

        [TestMethod]
        public void CorrespondingRowUpdated()
        {
            var conditions = new List<ConditionNode>() { new ConditionNode("c1", "=", "4") };

            table.Update(new List<string> { "c2" }, new List<string> { "10" }, conditions);
            var values2Updated = new List<string> { "4", "10", "6" };

            var result = table.Select(new List<string> { "*" }, new List<ConditionNode>());
            Assert.AreEqual(2, result.Count);
            CollectionAssert.AreEqual(values1, result[0]);
            CollectionAssert.AreEqual(values2Updated, result[1]);
        }
    }
}
