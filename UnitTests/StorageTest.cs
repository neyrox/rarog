using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using Engine;
using Engine.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class StorageTest
    {
        [TestMethod]
        public void MemoryStorageTest()
        {
            StoreAndLoad(new MemoryStorage());
        }

        [TestMethod]
        public void FileStorageTest()
        {
            StoreAndLoad(new FileStorage(Path.GetTempPath()));
        }

        private void StoreAndLoad(IStorage storage)
        {
            // Given
            {
                var db2Clean = new Database(storage);
                db2Clean.Load();
                if (db2Clean.ContainsTable("t1"))
                    db2Clean.RemoveTable("t1");
            }

            {
                var db2Store = new Database(storage);

                var t10 = db2Store.CreateTable("t1");
                t10.AddColumn("c_int", "int", 0);
                t10.AddColumn("c_lng", "bigint", 0);
                t10.AddColumn("c_dbl", "double", 0);
                t10.AddColumn("c_str", "varchar", 16);

                t10.Insert(
                    new List<string> {"c_int", "c_lng", "c_dbl", "c_str"},
                    new List<string> {"12345", "9876543210", "123.456", "abracodabra"});
                t10.Insert(
                    new List<string> {"c_int", "c_lng", "c_dbl", "c_str"},
                    new List<string> {int.MinValue.ToString(), long.MinValue.ToString(), double.MinValue.ToString(CultureInfo.InvariantCulture), "hmm..."});
                t10.Insert(
                    new List<string> {"c_int", "c_lng", "c_dbl", "c_str"},
                    new List<string> {int.MaxValue.ToString(), long.MaxValue.ToString(), double.MaxValue.ToString(CultureInfo.InvariantCulture), "WOW"});

                db2Store.Flush();
            }

            // When
            var db2Load = new Database(storage);
            db2Load.Load();

            //Then
            var t11 = db2Load.GetTable("t1");
            var c_int = t11.GetColumn("c_int").Get(new List<long> {0, 1, 2});
            CollectionAssert.AreEqual(new [] {12345, int.MinValue, int.MaxValue}, (c_int as ResultColumnInteger)?.Values );
            var c_lng = t11.GetColumn("c_lng").Get(new List<long> {0, 1, 2});
            CollectionAssert.AreEqual(new [] {9876543210, long.MinValue, long.MaxValue}, (c_lng as ResultColumnBigInt)?.Values );
            var c_dbl = t11.GetColumn("c_dbl").Get(new List<long> {0, 1, 2});
            var dblVals = (c_dbl as ResultColumnDouble)?.Values;
            Assert.IsNotNull(dblVals);
            Assert.AreEqual(123.456, dblVals[0]);
            Assert.AreEqual(double.MinValue, dblVals[1]);
            Assert.AreEqual(double.MaxValue, dblVals[2]);

            Assert.IsTrue(db2Load.RemoveTable("t1"));
        }
    }
}
