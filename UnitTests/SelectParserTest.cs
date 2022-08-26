using Engine;
using Engine.Statement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class SelectParserTest
    {
        [TestMethod]
        public void ParseSelect1()
        {
            var tokens = new [] {"SELECT", "1", ";" };
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(SelectWithoutTable));
            var select = (SelectWithoutTable)root;
            Assert.IsInstanceOfType(select.What[0], typeof(ValueNode));
            Assert.AreEqual(1, select.What.Count);
            Assert.AreEqual("1", ((ValueNode)select.What[0]).Item);
        }

        [TestMethod]
        public void ParseSimpleSelect()
        {
            var tokens = new string[] {"SELECT", "*", "FROM", "Customers", ";" };
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(SelectNode));
            var select = (SelectNode)root;
            Assert.IsInstanceOfType(select.What[0], typeof(ValueNode));
            Assert.AreEqual(1, select.What.Count);
            Assert.AreEqual("*", ((ValueNode)select.What[0]).Item);
            Assert.AreEqual("Customers", select.TableName);
        }

        [TestMethod]
        public void ParseSelectLimit()
        {
            var tokens = new string[] {"SELECT", "*", "FROM", "Agents", "LIMIT", "1", ";" };
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(SelectNode));
            var select = (SelectNode)root;
            Assert.IsInstanceOfType(select.What[0], typeof(ValueNode));
            Assert.AreEqual(1, select.What.Count);
            Assert.AreEqual("*", ((ValueNode)select.What[0]).Item);
            Assert.AreEqual("Agents", select.TableName);
            Assert.AreEqual(1, select.Limit);
        }

        [TestMethod]
        public void ParseSelectCount()
        {
            var tokens = new string[] {"SELECT", "COUNT", "(", "*", ")", "FROM", "Agents", ";" };
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(SelectNode));
            var select = (SelectNode)root;
            Assert.IsInstanceOfType(select.What[0], typeof(FunctionNode));
            Assert.AreEqual(1, select.What.Count);
            Assert.AreEqual("COUNT", ((FunctionNode)select.What[0]).Function);
            Assert.AreEqual("*", ((ValueNode)((FunctionNode)select.What[0]).Item).Item);
            Assert.AreEqual("Agents", select.TableName);
        }

        [TestMethod]
        public void ParseSelectWhere()
        {
            var tokens = new string[] { "SELECT", "*", "FROM", "Customers", "WHERE", "CustomerID", "=", "1", ";" };
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(SelectNode));
            var select = (SelectNode)root;
            Assert.IsInstanceOfType(select.What[0], typeof(ValueNode));
            Assert.AreEqual(1, select.What.Count);
            Assert.AreEqual("*", ((ValueNode)select.What[0]).Item);
            Assert.AreEqual("Customers", select.TableName);
            Assert.IsNotNull(select.Condition);
            var columnCondition = select.Condition as ColumnConditionNode;
            Assert.AreEqual("CustomerID", columnCondition.ColumnName);
            Assert.AreEqual("=", columnCondition.Operation);
            Assert.AreEqual("1", columnCondition.Value);
        }

        [TestMethod]
        public void ParseSelectWhereComposite2()
        {
            var tokens = new string[] { "SELECT", "*", "FROM", "t1", "WHERE", "c1", "=", "1", "AND", "c2", "=", "2", ";" };
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(SelectNode));
            var select = (SelectNode)root;
            Assert.IsInstanceOfType(select.What[0], typeof(ValueNode));
            Assert.AreEqual(1, select.What.Count);
            Assert.AreEqual("*", ((ValueNode)select.What[0]).Item);
            Assert.AreEqual("t1", select.TableName);
            Assert.IsNotNull(select.Condition);
            var compositeCondition = select.Condition as CompositeConditionNode;
            var columnCondition1 = compositeCondition.Left as ColumnConditionNode;
            Assert.AreEqual("c1", columnCondition1.ColumnName);
            Assert.AreEqual("=", columnCondition1.Operation);
            Assert.AreEqual("1", columnCondition1.Value);
            var columnCondition2 = compositeCondition.Right as ColumnConditionNode;
            Assert.AreEqual("c2", columnCondition2.ColumnName);
            Assert.AreEqual("=", columnCondition2.Operation);
            Assert.AreEqual("2", columnCondition2.Value);
        }

        [TestMethod]
        public void ParseSelectWhereComposite3()
        {
            var tokens = new string[] { "SELECT", "*", "FROM", "t1", "WHERE", "c1", "<", "1", "AND", "c2", "=", "2", "AND", "c3", ">", "3", ";" };
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(SelectNode));
            var select = (SelectNode)root;
            Assert.IsInstanceOfType(select.What[0], typeof(ValueNode));
            Assert.AreEqual(1, select.What.Count);
            Assert.AreEqual("*", ((ValueNode)select.What[0]).Item);
            Assert.AreEqual("t1", select.TableName);
            Assert.IsNotNull(select.Condition);
            var compositeCondition1 = select.Condition as CompositeConditionNode;

            var compositeCondition2 = compositeCondition1.Left as CompositeConditionNode;

            var columnCondition1 = compositeCondition2.Left as ColumnConditionNode;
            Assert.AreEqual("c1", columnCondition1.ColumnName);
            Assert.AreEqual("<", columnCondition1.Operation);
            Assert.AreEqual("1", columnCondition1.Value);

            var columnCondition2 = compositeCondition2.Right as ColumnConditionNode;
            Assert.AreEqual("c2", columnCondition2.ColumnName);
            Assert.AreEqual("=", columnCondition2.Operation);
            Assert.AreEqual("2", columnCondition2.Value);

            var columnCondition3 = compositeCondition1.Right as ColumnConditionNode;
            Assert.AreEqual("c3", columnCondition3.ColumnName);
            Assert.AreEqual(">", columnCondition3.Operation);
            Assert.AreEqual("3", columnCondition3.Value);
        }

        [TestMethod]
        public void ParseSelectTwoFields()
        {
            var tokens = new string[] { "SELECT", "CustomerName", ",", "Country", "FROM", "Customers", ";" };
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(SelectNode));
            var select = (SelectNode)root;
            Assert.AreEqual(2, select.What.Count);
            Assert.IsInstanceOfType(select.What[0], typeof(ValueNode));
            Assert.IsInstanceOfType(select.What[1], typeof(ValueNode));
            Assert.AreEqual("CustomerName", ((ValueNode)select.What[0]).Item);
            Assert.AreEqual("Country", ((ValueNode)select.What[1]).Item);
            Assert.AreEqual("Customers", select.TableName);
        }
    }
}
