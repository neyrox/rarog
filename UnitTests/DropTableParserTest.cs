using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class DropTableParserTest
    {
        [TestMethod]
        public void ParseDropTable()
        {
            var tokens = new []
            {
                "DROP", "TABLE",
                "Persons", ";"
            };
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(DropTableNode));
            var dropTable = (DropTableNode) root;
            Assert.AreEqual("Persons", dropTable.TableName);
            Assert.IsFalse(dropTable.IfExists);
        }

        [TestMethod]
        public void ParseDropTableIfExists()
        {
            var tokens = new []
            {
                "DROP", "table", "IF", "exists",
                "Persons", ";"
            };
            var root = Parser.Convert(tokens);
            Assert.IsInstanceOfType(root, typeof(DropTableNode));
            var dropTable = (DropTableNode) root;
            Assert.AreEqual("Persons", dropTable.TableName);
            Assert.IsTrue(dropTable.IfExists);
        }
    }
}
