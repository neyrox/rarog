using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Engine.Storage;

namespace UnitTests
{
    [TestClass]
    public class VarCharColumnStoreTest
    {
        private ColumnVarChar column1;
        private IStorage storage;

        [TestInitialize]
        public void Setup()
        {
            storage = new MemoryStorage();

            column1 = new ColumnVarChar("t1", "c1");
            column1.Insert(0, "000", storage);
            column1.Insert(1, "aaa", storage);
            column1.Insert(2, "bbb", storage);
            column1.Insert(5, "eee", storage);
            column1.Insert(7, "ggg", storage);
        }

        [TestMethod]
        public void StoredAndLoaded()
        {
            var column2 = new ColumnVarChar("t1", "c1");
            var whole1 = column1.Get(null, storage);
            var whole2 = column2.Get(null, storage);

            CollectionAssert.AreEqual(whole1.All(), whole2.All());
        }
    }
}
