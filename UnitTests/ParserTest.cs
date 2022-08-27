using Engine;
using Engine.Statement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class ParserTest
    {
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
        public void ParseDropTable()
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
        public void ParseInsertNegative()
        {
            var tokens = new string[] { "INSERT", "INTO", "Numbers",
                "(", "Value", ")", "VALUES", "(", "-", "100", ")", ";"};
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(InsertNode));
            var insert = (InsertNode)root;
            Assert.AreEqual("Numbers", insert.TableName);
            Assert.AreEqual(1, insert.ColumnNames.Count);
            Assert.AreEqual("Value", insert.ColumnNames[0]);
            Assert.AreEqual(1, insert.Values.Count);
            Assert.AreEqual("-100", insert.Values[0]);
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
            Assert.AreEqual(1, update.Ops.Count);
            Assert.AreEqual(OperationNode.Assign, update.Ops[0].Op);
            Assert.AreEqual("Alfred", update.Ops[0].Value);
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
            Assert.AreEqual(2, update.Ops.Count);
            Assert.AreEqual(OperationNode.Assign, update.Ops[0].Op);
            Assert.AreEqual("Alfred", update.Ops[0].Value);
            Assert.AreEqual(OperationNode.Assign, update.Ops[0].Op);
            Assert.AreEqual("Frankfurt", update.Ops[1].Value);
            Assert.IsNotNull(update.Condition);
            var columnCondition = update.Condition as ColumnConditionNode;
            Assert.AreEqual("CustomerID", columnCondition.ColumnName);
            Assert.AreEqual("=", columnCondition.Operation);
            Assert.AreEqual("1", columnCondition.Value);
        }
        
        [TestMethod]
        public void ParseFlush()
        {
            var tokens = new string[] { "FLUSH", ";"};
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(FlushNode));
        }

        [TestMethod]
        public void ParseBeginTransaction()
        {
            var tokens = new string[] { "BEGIN", ";"};
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(BeginTransactionNode));
        }

        [TestMethod]
        public void ParseEndTransaction()
        {
            var tokens = new string[] { "END", ";"};
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(EndTransactionNode));
        }
    }
}
