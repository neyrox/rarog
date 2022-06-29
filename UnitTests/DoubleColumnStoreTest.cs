using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Engine.Storage;

namespace UnitTests
{
    [TestClass]
    public class DoubleColumnStoreTest
    {
        private ColumnDouble column;
        private IStorage storage;

        [TestInitialize]
        public void Setup()
        {
            column = new ColumnDouble("c1");
            column.Insert(0, "100.1");
            column.Insert(1, "200.2");
            column.Insert(2, "300.3");
            column.Insert(5, "600.6");
            column.Insert(7, "800.8");

            storage = new MemoryStorage();
        }

        [TestMethod]
        public void StoredAndLoaded()
        {
            byte[] buffer;
            column.Store(storage, "path");

            var outColumn = storage.LoadColumn(Column.GetFileName("path", column.Name));

            Assert.AreEqual(column.Count, outColumn.Count);
            CollectionAssert.AreEqual(column.Indices.ToArray(), outColumn.Indices.ToArray());
            CollectionAssert.AreEqual(column.Get(null).All(), outColumn.Get(null).All());
        }
    }
}
