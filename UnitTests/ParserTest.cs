using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void ParseSimpleSelect()
        {
            var tokens = new string[] {"SELECT", "*", "FROM", "Customers", ";" };
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(SelectNode));
            var select = (SelectNode)root;
            Assert.AreEqual("*", select.What[0]);
            Assert.AreEqual("Customers", select.TableName);
        }

        [TestMethod]
        public void ParseSelectWhere()
        {
            var tokens = new string[] { "SELECT", "*", "FROM", "Customers", "WHERE", "CustomerID", "=", "1", ";" };
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(SelectNode));
            var select = (SelectNode)root;
            Assert.AreEqual("*", select.What[0]);
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
            Assert.AreEqual("*", select.What[0]);
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
            Assert.AreEqual("*", select.What[0]);
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
            Assert.AreEqual("CustomerName", select.What[0]);
            Assert.AreEqual("Country", select.What[1]);
            Assert.AreEqual("Customers", select.TableName);
        }

        [TestMethod]
        public void ParseSimpleCreateTable()
        {
            var tokens = new string[] { "CREATE", "TABLE", "Persons",
                "(", "id", "int", ",", "name", "varchar", "(", "255", ")", ")", ";"};
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(CreateTableNode));
            var create = (CreateTableNode)root;
            Assert.AreEqual("Persons", create.TableName);
            Assert.AreEqual(2, create.ColumnNames.Count);
            Assert.AreEqual("id", create.ColumnNames[0]);
            Assert.AreEqual("name", create.ColumnNames[1]);
            Assert.AreEqual(2, create.DataTypes.Count);
            Assert.AreEqual("int", create.DataTypes[0]);
            Assert.AreEqual("varchar", create.DataTypes[1]);
            Assert.AreEqual(2, create.Lengths.Count);
            Assert.AreEqual(0, create.Lengths[0]);
            Assert.AreEqual(255, create.Lengths[1]);
        }

        [TestMethod]
        public void ParseSropTable()
        {
            var tokens = new string[] { "DROP", "TABLE", "Persons", ";"};
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(DropTableNode));
            var drop = (DropTableNode)root;
            Assert.AreEqual("Persons", drop.TableName);
        }

        [TestMethod]
        public void ParseSimpleInsert()
        {
            var tokens = new string[] { "INSERT", "INTO", "Customers",
                "(", "CustomerName", ",", "Country", ")", "VALUES", "(", "Cardinal", ",", "Norway", ")", ";"};
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(InsertNode));
            var insert = (InsertNode)root;
            Assert.AreEqual("Customers", insert.TableName);
            Assert.AreEqual(2, insert.ColumnNames.Count);
            Assert.AreEqual("CustomerName", insert.ColumnNames[0]);
            Assert.AreEqual("Country", insert.ColumnNames[1]);
            Assert.AreEqual(2, insert.Values.Count);
            Assert.AreEqual("Cardinal", insert.Values[0]);
            Assert.AreEqual("Norway", insert.Values[1]);
        }

        [TestMethod]
        public void ParseSimpleUpdate()
        {
            var tokens = new string[] { "UPDATE", "Customers",
                "SET", "ContactName", "=", "Alfred", ";"};
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(UpdateNode));
            var update = (UpdateNode)root;
            Assert.AreEqual("Customers", update.TableName);
            Assert.AreEqual(1, update.ColumnNames.Count);
            Assert.AreEqual("ContactName", update.ColumnNames[0]);
            Assert.AreEqual(1, update.Values.Count);
            Assert.AreEqual("Alfred", update.Values[0]);
            Assert.IsNull(update.Condition);
        }

        [TestMethod]
        public void ParseUpdateWhere()
        {
            var tokens = new string[] { "UPDATE", "Customers",
                "SET", "ContactName", "=", "Alfred", ",", "City", "=", "Frankfurt", "WHERE", "CustomerID", "=", "1", ";"};
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(UpdateNode));
            var update = (UpdateNode)root;
            Assert.AreEqual("Customers", update.TableName);
            Assert.AreEqual(2, update.ColumnNames.Count);
            Assert.AreEqual("ContactName", update.ColumnNames[0]);
            Assert.AreEqual("City", update.ColumnNames[1]);
            Assert.AreEqual(2, update.Values.Count);
            Assert.AreEqual("Alfred", update.Values[0]);
            Assert.AreEqual("Frankfurt", update.Values[1]);
            Assert.IsNotNull(update.Condition);
            var columnCondition = update.Condition as ColumnConditionNode;
            Assert.AreEqual("CustomerID", columnCondition.ColumnName);
            Assert.AreEqual("=", columnCondition.Operation);
            Assert.AreEqual("1", columnCondition.Value);
        }
    }
}
