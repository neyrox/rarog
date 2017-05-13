using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine;
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
            Assert.AreEqual(2, select1.Rows.Count);
            CollectionAssert.AreEqual(new List<string> { "1", "2", "10" }, select1.Rows[0]);
            CollectionAssert.AreEqual(new List<string> { "3", "4", "30" }, select1.Rows[1]);

            shell.Execute("UPDATE t1 SET c2 = 8 WHERE c1 = 1;");
            var select2 = shell.Execute("SELECT * FROM t1;");
            Assert.AreEqual(2, select2.Rows.Count);
            CollectionAssert.AreEqual(new List<string> { "1", "8", "10" }, select2.Rows[0]);
            CollectionAssert.AreEqual(new List<string> { "3", "4", "30" }, select2.Rows[1]);
            
            // TODO: check success
            var drop = shell.Execute("DROP TABLE t1;");
            Assert.IsTrue(drop.IsOK);
        }
    }
}
