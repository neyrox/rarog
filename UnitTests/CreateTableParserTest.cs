using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class CreateTableParserTest
    {
        [TestMethod]
        public void ParseCreateTable()
        {
            var tokens = new []
            {
                "Create", "TABLE",
                "Persons", "(", "nick", "varchar", "(", "100", ")", ")", ";"
            };
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(CreateTableNode));
            var createTable = (CreateTableNode) root;
            Assert.AreEqual("Persons", createTable.TableName);
            CollectionAssert.AreEqual(new[] {100}, createTable.Lengths.ToArray());
            CollectionAssert.AreEqual(new[] {"nick"}, createTable.ColumnNames.ToArray());
            CollectionAssert.AreEqual(new[] {"varchar"}, createTable.DataTypes.ToArray());
            Assert.IsFalse(createTable.IfNotExists);
        }

        [TestMethod]
        public void ParseCreateTableIfNotExists()
        {
            var tokens = new []
            {
                "Create", "TABLE", "IF", "not", "exists",
                "t1", "(", "c1", "int", ")", ";"
            };
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(CreateTableNode));
            var createTable = (CreateTableNode) root;
            Assert.AreEqual("t1", createTable.TableName);
            CollectionAssert.AreEqual(new[] {0}, createTable.Lengths.ToArray());
            CollectionAssert.AreEqual(new[] {"c1"}, createTable.ColumnNames.ToArray());
            CollectionAssert.AreEqual(new[] {"int"}, createTable.DataTypes.ToArray());
            Assert.IsTrue(createTable.IfNotExists);
        }
    }
}
