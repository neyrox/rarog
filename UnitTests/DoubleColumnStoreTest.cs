using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Engine.Storage;

namespace UnitTests
{
    [TestClass]
    public class DoubleColumnStoreTest
    {
        private ColumnDouble column1;
        private IStorage storage;

        [TestInitialize]
        public void Setup()
        {
            storage = new MemoryStorage();

            column1 = new ColumnDouble("t1", "c1");
            column1.Insert(0, "100.1", storage);
            column1.Insert(1, "200.2", storage);
            column1.Insert(2, "300.3", storage);
            column1.Insert(5, "600.6", storage);
            column1.Insert(7, "800.8", storage);
        }

        [TestMethod]
        public void StoredAndLoaded()
        {
            var column2 = new ColumnDouble("t1", "c1");
            var whole1 = column1.Get(null, storage);
            var whole2 = column2.Get(null, storage);

            Assert.AreEqual(column1.Count, column2.Count);
            Assert.AreEqual(whole1.Count, whole2.Count);
            CollectionAssert.AreEqual(column1.Indices.ToArray(), column2.Indices.ToArray());
            CollectionAssert.AreEqual(whole1.All(), whole2.All());
        }
    }
}
