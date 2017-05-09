using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine;

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
            Assert.AreEqual("Customers", select.From);
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
            Assert.AreEqual("Customers", select.From);
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
        public void ParseSimpleInsert()
        {
            var tokens = new string[] { "INSERT", "INTO", "Customers",
                "(", "CustomerName", ",", "Country", ")", "VALUES", "(", "Cardinal", ",", "Norway", ")", ";"};
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(InsertNode));
            var insert = (InsertNode)root;
            Assert.AreEqual(2, insert.ColumnNames.Count);
            Assert.AreEqual("CustomerName", insert.ColumnNames[0]);
            Assert.AreEqual("Country", insert.ColumnNames[1]);
            Assert.AreEqual(2, insert.Values.Count);
            Assert.AreEqual("Cardinal", insert.Values[0]);
            Assert.AreEqual("Norway", insert.Values[1]);
        }
    }
}
