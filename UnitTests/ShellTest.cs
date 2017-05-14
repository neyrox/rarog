using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class ShellTest
    {
        [TestMethod]
        public void UseCase1()
        {
            var db = new Database();
            var shell = new Shell(db);

            var create = shell.Execute("CREATE TABLE t1 (c1 int, c2 int, c3 int);");
            Assert.IsTrue(create.IsOK);
            var insert1 = shell.Execute("INSERT INTO t1 (c1, c2, c3) VALUES (1, 2, 10);");
            Assert.IsTrue(insert1.IsOK);
            var insert2 = shell.Execute("INSERT INTO t1 (c1, c2, c3) VALUES (3, 4, 30);");
            Assert.IsTrue(insert2.IsOK);
            var select1 = shell.Execute("SELECT * FROM t1;");
            Assert.AreEqual(3, select1.Columns.Count);
            CollectionAssert.AreEqual(new List<string> { "1", "3" }, select1.Columns[0].All());
            CollectionAssert.AreEqual(new List<string> { "2", "4" }, select1.Columns[1].All());
            CollectionAssert.AreEqual(new List<string> { "10", "30" }, select1.Columns[2].All());

            shell.Execute("UPDATE t1 SET c2 = 8 WHERE c1 = 1;");
            var select2 = shell.Execute("SELECT * FROM t1;");
            Assert.AreEqual(3, select2.Columns.Count);
            CollectionAssert.AreEqual(new List<string> { "1", "3" }, select2.Columns[0].All());
            CollectionAssert.AreEqual(new List<string> { "8", "4" }, select2.Columns[1].All());
            CollectionAssert.AreEqual(new List<string> { "10", "30" }, select2.Columns[2].All());
            
            // TODO: check success
            var drop = shell.Execute("DROP TABLE t1;");
            Assert.IsTrue(drop.IsOK);
        }
    }
}
