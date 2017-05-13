using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine;

namespace UnitTests
{
    [TestClass]
    public class LexerTest
    {
        [TestMethod]
        public void LexerSplitsSimpleCreateTable()
        {
            var tokens = Lexer.Split("CREATE TABLE Persons (id int);");
            Assert.AreEqual("CREATE", tokens[0]);
            Assert.AreEqual("TABLE", tokens[1]);
            Assert.AreEqual("Persons", tokens[2]);
            Assert.AreEqual("(", tokens[3]);
            Assert.AreEqual("id", tokens[4]);
            Assert.AreEqual("int", tokens[5]);
            Assert.AreEqual(")", tokens[6]);
            Assert.AreEqual(";", tokens[7]);
        }

        [TestMethod]
        public void LexerSplitsSimpleDropTable()
        {
            var tokens = Lexer.Split("DROP TABLE Persons;");
            Assert.AreEqual("DROP", tokens[0]);
            Assert.AreEqual("TABLE", tokens[1]);
            Assert.AreEqual("Persons", tokens[2]);
            Assert.AreEqual(";", tokens[3]);
        }

        [TestMethod]
        public void LexerSplitsSimpleSelect()
        {
            var tokens = Lexer.Split("SELECT * FROM Customers;");
            Assert.AreEqual("SELECT", tokens[0]);
            Assert.AreEqual("*", tokens[1]);
            Assert.AreEqual("FROM", tokens[2]);
            Assert.AreEqual("Customers", tokens[3]);
            Assert.AreEqual(";", tokens[4]);
        }

        [TestMethod]
        public void LexerSplitsSimpleInsert()
        {
            var tokens = Lexer.Split("INSERT INTO Customers (CustomerName, Country) VALUES (\'Cardinal\', \'Norway\');");
            Assert.AreEqual("INSERT", tokens[0]);
            Assert.AreEqual("INTO", tokens[1]);
            Assert.AreEqual("Customers", tokens[2]);
            Assert.AreEqual("(", tokens[3]);
            Assert.AreEqual("CustomerName", tokens[4]);
            Assert.AreEqual(",", tokens[5]);
            Assert.AreEqual("Country", tokens[6]);
            Assert.AreEqual(")", tokens[7]);
            Assert.AreEqual("VALUES", tokens[8]);
            Assert.AreEqual("(", tokens[9]);
            Assert.AreEqual("Cardinal", tokens[10]);
            Assert.AreEqual(",", tokens[11]);
            Assert.AreEqual("Norway", tokens[12]);
            Assert.AreEqual(")", tokens[13]);
            Assert.AreEqual(";", tokens[14]);
        }

        [TestMethod]
        public void LexerSplitsSimpleUpdate()
        {
            var tokens = Lexer.Split("UPDATE Customers SET ContactName = \'Alfred\', City= \'Frankfurt\' WHERE CustomerID = 1;");
            Assert.AreEqual("UPDATE", tokens[0]);
            Assert.AreEqual("Customers", tokens[1]);
            Assert.AreEqual("SET", tokens[2]);
            Assert.AreEqual("ContactName", tokens[3]);
            Assert.AreEqual("=", tokens[4]);
            Assert.AreEqual("Alfred", tokens[5]);
            Assert.AreEqual(",", tokens[6]);
            Assert.AreEqual("City", tokens[7]);
            Assert.AreEqual("=", tokens[8]);
            Assert.AreEqual("Frankfurt", tokens[9]);
            Assert.AreEqual("WHERE", tokens[10]);
            Assert.AreEqual("CustomerID", tokens[11]);
            Assert.AreEqual("=", tokens[12]);
            Assert.AreEqual("1", tokens[13]);
            Assert.AreEqual(";", tokens[14]);
        }
    }
}
