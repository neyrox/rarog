using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Engine.Storage;

namespace UnitTests
{
    [TestClass]
    public class ShellTest
    {
        Database db;
        Shell shell;

        [TestInitialize]
        public void Setup()
        {
            db = new Database(new MemoryStorage());
            shell = new Shell(db);
        }

        [TestMethod]
        public void UseCase1()
        {
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
            var create = shell.Execute("CREATE TABLE t1 (name varchar(3));");
            Assert.IsTrue(create.IsOK);
            var insert1 = shell.Execute("INSERT INTO t1 (name) VALUES (pi);");
            Assert.IsTrue(insert1.IsOK);
            var insert2 = shell.Execute("INSERT INTO t1 (name) VALUES (e);");
            Assert.IsTrue(insert2.IsOK);
            var alter1 = shell.Execute("ALTER TABLE t1 ADD value double;");
            Assert.IsTrue(alter1.IsOK);
            var select1 = shell.Execute("SELECT * FROM t1 WHERE name = pi;");
            Assert.AreEqual(2, select1.Columns.Count);
            CollectionAssert.AreEqual(new List<string> { "pi" }, select1.Columns[0].All());
            CollectionAssert.AreEqual(new List<string> { "0" }, select1.Columns[1].All());

            shell.Execute("UPDATE t1 SET value = 3.14 WHERE name = pi;");
            shell.Execute("UPDATE t1 SET value = 2.72 WHERE name = e;");
            var select2 = shell.Execute("SELECT * FROM t1;");
            Assert.AreEqual(2, select2.Columns.Count);
            CollectionAssert.AreEqual(new List<string> { "pi", "e" }, select2.Columns[0].All());
            CollectionAssert.AreEqual(new List<string> { "3.14", "2.72" }, select2.Columns[1].All());

            var delete = shell.Execute("DELETE FROM t1 WHERE value > 3.0;");
            var select3 = shell.Execute("SELECT * FROM t1;");
            CollectionAssert.AreEqual(ToList("e"), select3.Columns[0].All());
            CollectionAssert.AreEqual(ToList("2.72"), select3.Columns[1].All());

            var insert3 = shell.Execute("INSERT INTO t1 (name, value) VALUES (phi, 1.62);");
            Assert.IsTrue(insert3.IsOK);
            var alter2 = shell.Execute("ALTER TABLE t1 DROP COLUMN value;");
            Assert.IsTrue(alter2.IsOK);
            var select4 = shell.Execute("SELECT * FROM t1;");
            Assert.AreEqual(1, select4.Columns.Count);
            CollectionAssert.AreEqual(new List<string> { "e", "phi" }, select4.Columns[0].All());

            var drop = shell.Execute("DROP TABLE t1;");
            Assert.IsTrue(drop.IsOK);
        }

        private List<string> ToList(string item)
        {
            return new List<string> { item };
        }
    }
}
