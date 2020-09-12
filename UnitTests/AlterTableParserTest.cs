using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class AlterTableParserTest
    {
        [TestMethod]
        public void ParseAlterTableAddDoubleColumn()
        {
            var tokens = new string[]
            {
                "ALTER", "TABLE", "Persons",
                "ADD", "weight", "double", ";"
            };
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(AlterTableAddColumnNode));
            var addColumn = (AlterTableAddColumnNode) root;
            Assert.AreEqual("Persons", addColumn.TableName);
            Assert.AreEqual("weight", addColumn.ColumnName);
            Assert.AreEqual("double", addColumn.DataType);
            Assert.AreEqual(0, addColumn.Length);
        }

        [TestMethod]
        public void ParseAlterTableAddVarCharColumn()
        {
            var tokens = new string[]
            {
                "ALTER", "TABLE", "Persons",
                "ADD", "nick", "varchar", "(",  "100", ")", ";"
            };
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(AlterTableAddColumnNode));
            var addColumn = (AlterTableAddColumnNode)root;
            Assert.AreEqual("Persons", addColumn.TableName);
            Assert.AreEqual("nick", addColumn.ColumnName);
            Assert.AreEqual("varchar", addColumn.DataType);
            Assert.AreEqual(100, addColumn.Length);
        }
    }
}
