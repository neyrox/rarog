﻿using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void LexerSplitsAlterTableAddColumn()
        {
            var tokens = Lexer.Split("ALTER TABLE Persons ADD weight double;");
            Assert.AreEqual("ALTER", tokens[0]);
            Assert.AreEqual("TABLE", tokens[1]);
            Assert.AreEqual("Persons", tokens[2]);
            Assert.AreEqual("ADD", tokens[3]);
            Assert.AreEqual("weight", tokens[4]);
            Assert.AreEqual("double", tokens[5]);
            Assert.AreEqual(";", tokens[6]);
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
        public void LexerSplitsMultiConditionSelect()
        {
            var tokens = Lexer.Split("SELECT * FROM t1 WHERE c1 >= 1 and c2 <= 10;");
            Assert.AreEqual("SELECT", tokens[0]);
            Assert.AreEqual("*", tokens[1]);
            Assert.AreEqual("FROM", tokens[2]);
            Assert.AreEqual("t1", tokens[3]);
            Assert.AreEqual("WHERE", tokens[4]);
            Assert.AreEqual("c1", tokens[5]);
            Assert.AreEqual(">=", tokens[6]);
            Assert.AreEqual("1", tokens[7]);
            Assert.AreEqual("and", tokens[8]);
            Assert.AreEqual("c2", tokens[9]);
            Assert.AreEqual("<=", tokens[10]);
            Assert.AreEqual("10", tokens[11]);
            Assert.AreEqual(";", tokens[12]);
        }

        [TestMethod]
        public void LexerSplitsSimpleInsert()
        {
            var tokens = Lexer.Split("INSERT INTO Customers (CustomerName, Country) VALUES (\'Cardinal Richelieu\', \'Norway\');");
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
            Assert.AreEqual("Cardinal Richelieu", tokens[10]);
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

        [TestMethod]
        public void LexerSplitsExpression()
        {
            var tokens = Lexer.Split("1+2");
            Assert.AreEqual("1", tokens[0]);
            Assert.AreEqual("+", tokens[1]);
            Assert.AreEqual("2", tokens[2]);
        }
    }
}
