using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Engine.Storage;

namespace UnitTests
{
    [TestClass]
    public class IntColumnStoreTest
    {
        private ColumnInteger column;
        private IStorage storage;

        [TestInitialize]
        public void Setup()
        {
            column = new ColumnInteger("c1");
            column.Insert(0, "100");
            column.Insert(1, "200");
            column.Insert(2, "300");
            column.Insert(5, "600");
            column.Insert(7, "800");

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
