using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Globalization;
using Engine.Storage;

namespace UnitTests
{
    [TestClass]
    public class OpsTest
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
        [DataRow("int", "100", "+", "1", "101")]
        [DataRow("int", "100", "-", "1", "99")]
        [DataRow("int", "5", "*", "3", "15")]
        [DataRow("int", "10", "*", "1.6", "16")]
        [DataRow("int", "42", "/", "7", "6")]
        [DataRow("int", "10", "/", "3", "3")]
        [DataRow("bigint", "100", "+", "1", "101")]
        [DataRow("bigint", "100", "-", "1", "99")]
        [DataRow("bigint", "5", "*", "3", "15")]
        [DataRow("bigint", "10", "*", "1.6", "16")]
        [DataRow("bigint", "42", "/", "7", "6")]
        [DataRow("bigint", "10", "/", "3", "3")]
        [DataRow("varchar(5)", "10", "+", "20", "1020")]
        public void UpdateWithOp(string cType, string src, string op, string val, string dst)
        {
            var create = shell.Execute($"CREATE TABLE t1 (c1 int, c2 {cType});");
            Assert.IsTrue(create.IsOK);
            var insert1 = shell.Execute($"INSERT INTO t1 (c1, c2) VALUES (1, {src});");
            Assert.IsTrue(insert1.IsOK);

            shell.Execute($"UPDATE t1 SET c2 = c2 {op} {val} WHERE c1 = 1;");
            var select1 = shell.Execute("SELECT * FROM t1;");
            Assert.AreEqual(2, select1.Columns.Count);
            CollectionAssert.AreEqual(new List<string> { "1" }, select1.Columns[0].All());
            CollectionAssert.AreEqual(new List<string> { dst }, select1.Columns[1].All());
        }

        [TestMethod]
        [DataRow("1.2", "+", "3.4", 4.6)]
        [DataRow("5.7", "-", "2.3", 3.4)]
        [DataRow("2.2", "*", "3.3", 7.26)]
        [DataRow("24.2", "/", "4.4", 5.5)]
        public void UpdateFloatWithOp(string src, string op, string val, double dst)
        {
            var create = shell.Execute($"CREATE TABLE t1 (c1 int, c2 double);");
            Assert.IsTrue(create.IsOK);
            var insert1 = shell.Execute($"INSERT INTO t1 (c1, c2) VALUES (1, {src});");
            Assert.IsTrue(insert1.IsOK);

            shell.Execute($"UPDATE t1 SET c2 = c2 {op} {val} WHERE c1 = 1;");
            var select1 = shell.Execute("SELECT * FROM t1;");
            Assert.AreEqual(2, select1.Columns.Count);
            CollectionAssert.AreEqual(new List<string> { "1" }, select1.Columns[0].All());
            Assert.AreEqual(dst, double.Parse(select1.Columns[1].Get(0), NumberFormatInfo.InvariantInfo), 0.001);
        }
    }
}
