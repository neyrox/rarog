using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Engine.Storage;

namespace UnitTests
{
    [TestClass]
    public class TableTest
    {
        private Table table;
        private readonly List<string> values1 = new List<string> { "1", "2.3", "a" };
        private readonly List<string> values2 = new List<string> { "4", "5.4", "b" };

        private readonly List<string> col1 = new List<string> { "1", "4" };
        private readonly List<string> col2 = new List<string> { "2.3", "5.4" };
        private readonly List<string> col3 = new List<string> { "a", "b" };

        [TestInitialize]
        public void Setup()
        {
            table = new Table("t1", new MemoryStorage());
            table.AddColumn("c1", "int", 0);
            table.AddColumn("c2", "double", 0);
            table.AddColumn("c3", "varchar", 255);
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
            var condition = new ColumnConditionNode("c1", "=", "4");

            var result = table.Select(new List<string> { "*" }, condition);
            Assert.AreEqual(3, result.Count);
            CollectionAssert.AreEqual(ToList("4"), result[0].All());
            CollectionAssert.AreEqual(ToList("5.4"), result[1].All());
            CollectionAssert.AreEqual(ToList("b"), result[2].All());
        }

        [TestMethod]
        public void RowWhereIntEqualUpdated()
        {
            var condition = new ColumnConditionNode("c1", "=", "4");

            table.Update(new List<string> { "c2" }, new List<string> { "10.1" }, condition);

            var result = table.Select(new List<string> { "*" }, null);
            Assert.AreEqual(3, result.Count);
            CollectionAssert.AreEqual(col1, result[0].All());
            CollectionAssert.AreEqual(new List<string> { "2.3", "10.1" }, result[1].All());
            CollectionAssert.AreEqual(col3, result[2].All());
        }

        [TestMethod]
        public void RowWhereStringEqualUpdated()
        {
            var condition = new ColumnConditionNode("c3", "=", "a");

            table.Update(new List<string> { "c2" }, new List<string> { "10.2" }, condition);

            var result = table.Select(new List<string> { "*" }, null);
            Assert.AreEqual(3, result.Count);
            CollectionAssert.AreEqual(col1, result[0].All());
            CollectionAssert.AreEqual(new List<string> { "10.2", "5.4" }, result[1].All());
            CollectionAssert.AreEqual(col3, result[2].All());
        }

        [TestMethod]
        public void OneRowDeleted()
        {
            table.Delete(new ColumnConditionNode("c1", "=", "1"));
            var result = table.Select(new List<string> { "*" }, null);
            Assert.AreEqual(3, result.Count);
            CollectionAssert.AreEqual(ToList("4"), result[0].All());
            CollectionAssert.AreEqual(ToList("5.4"), result[1].All());
            CollectionAssert.AreEqual(ToList("b"), result[2].All());
        }

        private List<string> ToList(string item)
        {
            return new List<string> { item };
        }
    }
}
