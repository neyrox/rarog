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

            var create = shell.Execute("CREATE TABLE t1 (c1 int, c2 int);");
            Assert.IsTrue(create.IsOK);
            var alter1 = shell.Execute("ALTER TABLE t1 ADD c3 varchar(255);");
            Assert.IsTrue(alter1.IsOK);
            var insert1 = shell.Execute("INSERT INTO t1 (c1, c2, c3) VALUES (1, 2, a);");
            Assert.IsTrue(insert1.IsOK);
            var insert2 = shell.Execute("INSERT INTO t1 (c1, c2, c3) VALUES (3, 2, b);");
            Assert.IsTrue(insert2.IsOK);
            var select1 = shell.Execute("SELECT * FROM t1 WHERE c2 = 2 and c3 = a;");
            Assert.AreEqual(3, select1.Columns.Count);
            CollectionAssert.AreEqual(new List<string> { "1" }, select1.Columns[0].All());
            CollectionAssert.AreEqual(new List<string> { "2" }, select1.Columns[1].All());
            CollectionAssert.AreEqual(new List<string> { "a" }, select1.Columns[2].All());

            shell.Execute("UPDATE t1 SET c2 = 8 WHERE c1 = 1;");
            var select2 = shell.Execute("SELECT * FROM t1;");
            Assert.AreEqual(3, select2.Columns.Count);
            CollectionAssert.AreEqual(new List<string> { "1", "3" }, select2.Columns[0].All());
            CollectionAssert.AreEqual(new List<string> { "8", "2" }, select2.Columns[1].All());
            CollectionAssert.AreEqual(new List<string> { "a", "b" }, select2.Columns[2].All());

            var delete = shell.Execute("DELETE FROM t1 WHERE c1 = 1;");
            var select3 = shell.Execute("SELECT * FROM t1;");
            CollectionAssert.AreEqual(ToList("3"), select3.Columns[0].All());
            CollectionAssert.AreEqual(ToList("2"), select3.Columns[1].All());
            CollectionAssert.AreEqual(ToList("b"), select3.Columns[2].All());

            var insert3 = shell.Execute("INSERT INTO t1 (c1, c2, c3) VALUES (1, 2, a);");
            Assert.IsTrue(insert3.IsOK);
            var select4 = shell.Execute("SELECT * FROM t1;");
            Assert.AreEqual(3, select4.Columns.Count);
            foreach (var column in select4.Columns)
            {
                Assert.AreEqual(2, column.Count);
            }

            var delete2 = shell.Execute("DELETE FROM t1 WHERE c3 = b;");
            var select5 = shell.Execute("SELECT * FROM t1;");
            CollectionAssert.AreEqual(ToList("1"), select5.Columns[0].All());
            CollectionAssert.AreEqual(ToList("2"), select5.Columns[1].All());
            CollectionAssert.AreEqual(ToList("a"), select5.Columns[2].All());

            var drop = shell.Execute("DROP TABLE t1;");
            Assert.IsTrue(drop.IsOK);
        }

        [TestMethod]
        public void UseCase2()
        {
            var db = new Database();
            var shell = new Shell(db);

            var create = shell.Execute("CREATE TABLE t1 (c1 int);");
            Assert.IsTrue(create.IsOK);
            var insert1 = shell.Execute("INSERT INTO t1 (c1) VALUES (1);");
            Assert.IsTrue(insert1.IsOK);
            var insert2 = shell.Execute("INSERT INTO t1 (c1) VALUES (2);");
            Assert.IsTrue(insert2.IsOK);
            var alter = shell.Execute("ALTER TABLE t1 ADD c2 double;");
            Assert.IsTrue(insert1.IsOK);
            var select1 = shell.Execute("SELECT * FROM t1 WHERE c1 = 1;");
            Assert.AreEqual(2, select1.Columns.Count);
            CollectionAssert.AreEqual(new List<string> { "1" }, select1.Columns[0].All());
            CollectionAssert.AreEqual(new List<string> { "0" }, select1.Columns[1].All());

            shell.Execute("UPDATE t1 SET c2 = 3.14 WHERE c1 = 1;");
            var select2 = shell.Execute("SELECT * FROM t1;");
            Assert.AreEqual(2, select2.Columns.Count);
            CollectionAssert.AreEqual(new List<string> { "1", "2" }, select2.Columns[0].All());
            CollectionAssert.AreEqual(new List<string> { "3.14", "0" }, select2.Columns[1].All());

            var delete = shell.Execute("DELETE FROM t1 WHERE c1 = 1;");
            var select3 = shell.Execute("SELECT * FROM t1;");
            CollectionAssert.AreEqual(ToList("2"), select3.Columns[0].All());
            CollectionAssert.AreEqual(ToList("0"), select3.Columns[1].All());

            var drop = shell.Execute("DROP TABLE t1;");
            Assert.IsTrue(drop.IsOK);
        }

        private List<string> ToList(string item)
        {
            return new List<string> { item };
        }
    }
}
