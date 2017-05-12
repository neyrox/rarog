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

            shell.Execute("CREATE TABLE t1 (c1 int, c2 int, c3 int);");
            shell.Execute("INSERT INTO t1 (c1, c2, c3) VALUES (1, 2, 10);");
            shell.Execute("INSERT INTO t1 (c1, c2, c3) VALUES (3, 4, 30);");
            var select1 = shell.Execute("SELECT * FROM t1;");
            Assert.AreEqual(2, select1.Count);
            CollectionAssert.AreEqual(new List<string> { "1", "2", "10"}, select1[0]);
            CollectionAssert.AreEqual(new List<string> { "3", "4", "30" }, select1[1]);
            shell.Execute("UPDATE t1 SET c2 = 8 WHERE c1 = 1;");
            var select2 = shell.Execute("SELECT * FROM t1;");
            Assert.AreEqual(2, select2.Count);
            CollectionAssert.AreEqual(new List<string> { "1", "8", "10" }, select2[0]);
            CollectionAssert.AreEqual(new List<string> { "3", "4", "30" }, select2[1]);
        }
    }
}
