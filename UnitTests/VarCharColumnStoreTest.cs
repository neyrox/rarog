using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Engine.Storage;

namespace UnitTests
{
    [TestClass]
    public class VarCharColumnStoreTest
    {
        private ColumnVarChar column;
        private IStorage storage;

        [TestInitialize]
        public void Setup()
        {
            column = new ColumnVarChar("c1");
            column.Insert(0, "000");
            column.Insert(1, "aaa");
            column.Insert(2, "bbb");
            column.Insert(5, "eee");
            column.Insert(7, "ggg");

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
