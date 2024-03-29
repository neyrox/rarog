﻿using Engine;
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
        [DataRow("SELECT 1;", "1")]
        [DataRow("SELECT 9000000000;", "9000000000")]
        [DataRow("SELECT 1 + 2;", "3")]
        [DataRow("SELECT 5000000000+7000000000;", "12000000000")]
        [DataRow("SELECT 100 - 42;", "58")]
        [DataRow("SELECT 12000000000-7000000000;", "5000000000")]
        [DataRow("SELECT 3 * 5;", "15")]
        [DataRow("SELECT 10000000000*2;", "20000000000")]
        [DataRow("SELECT 12 / 4;", "3")]
        [DataRow("SELECT 30000000000/3;", "10000000000")]
        [DataRow("SELECT a+b;", "ab")]
        public void CalculatorTest(string query, string res)
        {
            var select = shell.Execute(query);
            Assert.AreEqual(1, select.Columns.Count);
            Assert.AreEqual(1, select.Columns[0].Count);
            Assert.AreEqual(res, select.Columns[0].Get(0));
        }

        [TestMethod]
        [DataRow("SELECT 1.2;", 1.2)]
        [DataRow("SELECT 1.2+2.3;", 3.5)]
        [DataRow("SELECT 9.9 - 5.3;", 4.6)]
        [DataRow("SELECT 5 * 1.2;", 6)]
        [DataRow("SELECT 12.4 / 4;", 3.1)]
        public void DoubleCalculatorTest(string query, double res)
        {
            var select = shell.Execute(query);
            Assert.AreEqual(1, select.Columns.Count);
            Assert.AreEqual(res, ((ResultColumnDouble)select.Columns[0])[0], 0.001);
        }

        [TestMethod]
        public void CombinationTest()
        {
            // Given
            var create = shell.Execute("CREATE TABLE t1 (c1 int);");
            Assert.IsTrue(create.IsOK);
            var insert1 = shell.Execute("INSERT INTO t1 (c1) VALUES (1);");
            Assert.IsTrue(insert1.IsOK);
            var insert2 = shell.Execute("INSERT INTO t1 (c1) VALUES (2);");
            Assert.IsTrue(insert2.IsOK);
            var insert3 = shell.Execute("INSERT INTO t1 (c1) VALUES (3);");
            Assert.IsTrue(insert3.IsOK);

            var select1 = shell.Execute("select c1, abc from t1;");
            Assert.IsTrue(select1.IsOK);
            Assert.AreEqual(2, select1.Columns.Count);
            CollectionAssert.AreEqual(new [] { 1, 2, 3 }, ((ResultColumnInteger)select1.Columns[0]).Values);
            CollectionAssert.AreEqual(new List<string> { "abc", "abc", "abc" }, select1.Columns[1].All());

            var select2 = shell.Execute("select c1 + 3 from t1;");
            Assert.IsTrue(select2.IsOK);
            Assert.AreEqual(1, select2.Columns.Count);
            CollectionAssert.AreEqual(new [] { 4, 5, 6 }, ((ResultColumnInteger)select2.Columns[0]).Values);
        }

        [TestMethod]
        public void FunctionsTest()
        {
            // Given
            var create = shell.Execute("CREATE TABLE t1 (c_int int, c_lng bigint, c_dbl double, c_str varchar(3));");
            Assert.IsTrue(create.IsOK);
            var insert1 = shell.Execute("INSERT INTO t1 (c_int, c_lng, c_dbl, c_str) VALUES (1, 11, 4.0, aaa);");
            Assert.IsTrue(insert1.IsOK);
            var insert2 = shell.Execute("INSERT INTO t1 (c_int, c_lng, c_dbl, c_str) VALUES (2, 7, 2.0, ccc);");
            Assert.IsTrue(insert2.IsOK);
            var insert3 = shell.Execute("INSERT INTO t1 (c_int, c_lng, c_dbl, c_str) VALUES (3, 5, 6.0, bbb);");
            Assert.IsTrue(insert3.IsOK);

            // When
            var selectCount = shell.Execute("select COUNT(*) from t1;");
            // Then
            Assert.IsTrue(selectCount.IsOK);
            Assert.AreEqual(1, selectCount.Columns.Count);
            var column1 = (ResultColumnBigInt) selectCount.Columns[0];
            Assert.AreEqual(1, column1.Count);
            Assert.AreEqual(3, column1.Values[0]);

            // When
            var selectMin = shell.Execute("select MIN(c_int), MIN(c_lng), MIN(c_dbl), MIN(c_str) from t1;");
            // Then
            Assert.IsTrue(selectMin.IsOK);
            Assert.AreEqual(4, selectMin.Columns.Count);
            CollectionAssert.AreEqual(new [] {1}, ((ResultColumnInteger)selectMin.Columns[0]).Values);
            CollectionAssert.AreEqual(new [] {5L}, ((ResultColumnBigInt)selectMin.Columns[1]).Values);
            Assert.AreEqual(2.0, ((ResultColumnDouble)selectMin.Columns[2]).Values[0]);
            CollectionAssert.AreEqual(new [] {"aaa"}, ((ResultColumnString)selectMin.Columns[3]).Values);

            // When
            var selectMax = shell.Execute("select MAX(c_int), MAX(c_lng), MAX(c_dbl), MAX(c_str) from t1;");
            // Then
            Assert.IsTrue(selectMax.IsOK);
            Assert.AreEqual(4, selectMax.Columns.Count);
            CollectionAssert.AreEqual(new [] {3}, ((ResultColumnInteger)selectMax.Columns[0]).Values);
            CollectionAssert.AreEqual(new [] {11L}, ((ResultColumnBigInt)selectMax.Columns[1]).Values);
            Assert.AreEqual(6.0, ((ResultColumnDouble)selectMax.Columns[2]).Values[0], 0.0001);
            CollectionAssert.AreEqual(new [] {"ccc"}, ((ResultColumnString)selectMax.Columns[3]).Values);
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
            var flush1 = shell.Execute("FLUSH;");
            Assert.IsTrue(flush1.IsOK);
            var select1 = shell.Execute("SELECT * FROM t1 WHERE c2 = 2 and c3 = a;");
            Assert.AreEqual(3, select1.Columns.Count);
            CollectionAssert.AreEqual(new List<string> { "1" }, select1.Columns[0].All());
            CollectionAssert.AreEqual(new List<string> { "2" }, select1.Columns[1].All());
            CollectionAssert.AreEqual(new List<string> { "a" }, select1.Columns[2].All());

            shell.Execute("UPDATE t1 SET c2 = 8 WHERE c1 = 1;");
            var select20 = shell.Execute("SELECT * FROM t1;");
            Assert.AreEqual(3, select20.Columns.Count);
            CollectionAssert.AreEqual(new List<string> { "1", "3" }, select20.Columns[0].All());
            CollectionAssert.AreEqual(new List<string> { "8", "2" }, select20.Columns[1].All());
            CollectionAssert.AreEqual(new List<string> { "a", "b" }, select20.Columns[2].All());

            shell.Execute("UPDATE t1 SET c2 = c2 + 2 WHERE c1 = 1;");
            var select21 = shell.Execute("SELECT * FROM t1;");
            Assert.AreEqual(3, select21.Columns.Count);
            CollectionAssert.AreEqual(new List<string> { "1", "3" }, select21.Columns[0].All());
            CollectionAssert.AreEqual(new List<string> { "10", "2" }, select21.Columns[1].All());
            CollectionAssert.AreEqual(new List<string> { "a", "b" }, select21.Columns[2].All());

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

            var flush1 = shell.Execute("FLUSH;");
            Assert.IsTrue(flush1.IsOK);

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

        [TestMethod]
        public void UseCase3()
        {
            var select1 = shell.Execute("SELECT * FROM t1;");
            Assert.IsFalse(select1.IsOK);

            var create = shell.Execute("CREATE TABLE t1 (c1 int);");
            Assert.IsTrue(create.IsOK);

            var select2 = shell.Execute("SELECT * FROM t1;");
            Assert.IsTrue(select2.IsOK);

            var drop = shell.Execute("DROP TABLE t1;");
            Assert.IsTrue(drop.IsOK);

            var select3 = shell.Execute("SELECT * FROM t1;");
            Assert.IsFalse(select3.IsOK);
        }

        private List<string> ToList(string item)
        {
            return new List<string> { item };
        }
    }
}
