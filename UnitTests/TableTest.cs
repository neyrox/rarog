using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Engine.Statement;
using Engine.Storage;

namespace UnitTests
{
    [TestClass]
    public class TableTest
    {
        private Table table;
        private readonly List<string> values1 = new List<string> { "1", "2.3", "10", "a" };
        private readonly List<string> values2 = new List<string> { "4", "5.4", "40", "b" };

        private readonly List<string> col1 = new List<string> { "1", "4" };
        private readonly List<string> col2 = new List<string> { "2.3", "5.4" };
        private readonly List<string> col3 = new List<string> { "10", "40" };
        private readonly List<string> col4 = new List<string> { "a", "b" };

        [TestInitialize]
        public void Setup()
        {
            table = new Table("t1", new MemoryStorage());
            table.AddColumn("c1", "int", 0);
            table.AddColumn("c2", "double", 0);
            table.AddColumn("c3", "bigint", 0);
            table.AddColumn("c4", "varchar", 255);
            var columnNames = new List<string> { "c1", "c2", "c3", "c4" };
            table.Insert(columnNames, values1);
            table.Insert(columnNames, values2);
        }

        [TestMethod]
        public void SelectAll()
        {
            var result = table.Select(new List<string> { "*" }, new AnyConditionNode(), 0);
            Assert.AreEqual(4, result.Count);
            CollectionAssert.AreEqual(col1, result[0].All());
            CollectionAssert.AreEqual(col2, result[1].All());
            CollectionAssert.AreEqual(col3, result[2].All());
            CollectionAssert.AreEqual(col4, result[3].All());
        }

        [TestMethod]
        public void SelectWithLimit()
        {
            var result = table.Select(new List<string> { "*" }, new AnyConditionNode(), 1);
            Assert.AreEqual(4, result.Count);
            CollectionAssert.AreEqual(col1.Take(1).ToList(), result[0].All());
            CollectionAssert.AreEqual(col2.Take(1).ToList(), result[1].All());
            CollectionAssert.AreEqual(col3.Take(1).ToList(), result[2].All());
            CollectionAssert.AreEqual(col4.Take(1).ToList(), result[3].All());
        }

        [TestMethod]
        public void CorrespondingRowSelected()
        {
            var condition = new ColumnConditionNode("c1", "=", "4");

            var result = table.Select(new List<string> { "*" }, condition, 0);
            Assert.AreEqual(4, result.Count);
            CollectionAssert.AreEqual(ToList("4"), result[0].All());
            CollectionAssert.AreEqual(ToList("5.4"), result[1].All());
            CollectionAssert.AreEqual(ToList("40"), result[2].All());
            CollectionAssert.AreEqual(ToList("b"), result[3].All());
        }

        [TestMethod]
        public void SelectWithSingleConditionLimit()
        {
            var condition = new ColumnConditionNode("c1", ">", "0");

            var result = table.Select(new List<string> { "*" }, condition, 1);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(1, result[0].Count);
            Assert.AreEqual(1, result[1].Count);
            Assert.AreEqual(1, result[2].Count);
            Assert.AreEqual(1, result[3].Count);
        }

        [TestMethod]
        [DataRow("AND")]
        [DataRow("OR")]
        public void SelectWithCompositeConditionLimit(string op)
        {
            var leftCond = new ColumnConditionNode("c1", ">", "0");
            var rightCond = new ColumnConditionNode("c2", ">", "0.0");
            var condition = new CompositeConditionNode(leftCond, op, rightCond);

            var result = table.Select(new List<string> { "*" }, condition, 1);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(1, result[0].Count);
            Assert.AreEqual(1, result[1].Count);
            Assert.AreEqual(1, result[2].Count);
            Assert.AreEqual(1, result[3].Count);
        }

        [TestMethod]
        public void RowWhereIntEqualUpdated()
        {
            var condition = new ColumnConditionNode("c1", "=", "4");
            var ops = new List<OperationNode> {new OperationNode {Op = OperationNode.Assign, Value = "10.1"}};

            table.Update(new List<string> { "c2" }, ops, condition);

            var result = table.Select(new List<string> { "*" }, new AnyConditionNode(), 0);
            Assert.AreEqual(4, result.Count);
            CollectionAssert.AreEqual(col1, result[0].All());
            CollectionAssert.AreEqual(new List<string> { "2.3", "10.1" }, result[1].All());
            CollectionAssert.AreEqual(col3, result[2].All());
            CollectionAssert.AreEqual(col4, result[3].All());
        }

        [TestMethod]
        public void RowWhereStringEqualUpdated()
        {
            var condition = new ColumnConditionNode("c4", "=", "a");
            var ops = new List<OperationNode> {new OperationNode {Op = OperationNode.Assign, Value = "10.2"}};

            table.Update(new List<string> { "c2" }, ops, condition);

            var result = table.Select(new List<string> { "*" }, new AnyConditionNode(), 0);
            Assert.AreEqual(4, result.Count);
            CollectionAssert.AreEqual(col1, result[0].All());
            CollectionAssert.AreEqual(new List<string> { "10.2", "5.4" }, result[1].All());
            CollectionAssert.AreEqual(col3, result[2].All());
            CollectionAssert.AreEqual(col4, result[3].All());
        }

        [TestMethod]
        public void OneRowDeleted()
        {
            table.Delete(new ColumnConditionNode("c1", "=", "1"));
            var result = table.Select(new List<string> { "*" }, new AnyConditionNode(), 0);
            Assert.AreEqual(4, result.Count);
            CollectionAssert.AreEqual(ToList("4"), result[0].All());
            CollectionAssert.AreEqual(ToList("5.4"), result[1].All());
            CollectionAssert.AreEqual(ToList("40"), result[2].All());
            CollectionAssert.AreEqual(ToList("b"), result[3].All());
        }

        private List<string> ToList(string item)
        {
            return new List<string> { item };
        }
    }
}
