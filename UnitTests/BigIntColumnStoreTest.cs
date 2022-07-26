using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine.Storage;

namespace UnitTests
{
    [TestClass]
    public class BigIntColumnStoreTest
    {
        private IStorage storage;
        private Registry registry;
        private ColumnBase<long> column1;

        [TestInitialize]
        public void Setup()
        {
            storage = new MemoryStorage();
            registry = new Registry(storage);

            column1 = new ColumnBase<long>("t1", "c1", registry.BigIntTraits);
            column1.Insert(0, "100");
            column1.Insert(1, "200");
            column1.Insert(2, "300");
            column1.Insert(5, "600");
            column1.Insert(7, "800");

            registry.BigIntTraits.PageStorage.Flush();
        }

        [TestMethod]
        public void AllSelected()
        {
            var column2 = new ColumnBase<long>("t1", "c1", registry.BigIntTraits);
            var whole1 = column1.Get(null);
            var whole2 = column2.Get(null);

            CollectionAssert.AreEqual(whole1.All(), whole2.All());
        }
    }
}
