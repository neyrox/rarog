using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine.Storage;

namespace UnitTests
{
    [TestClass]
    public class DoubleColumnStoreTest
    {
        private IStorage storage;
        private Registry registry;
        private ColumnBase<double> column1;

        [TestInitialize]
        public void Setup()
        {
            storage = new MemoryStorage();
            registry = new Registry(storage);

            column1 = new ColumnBase<double>("t1", "c1", registry.DoubleTraits);
            column1.Insert(0, "100.1");
            column1.Insert(1, "200.2");
            column1.Insert(2, "300.3");
            column1.Insert(5, "600.6");
            column1.Insert(7, "800.8");

            registry.DoubleTraits.PageStorage.Flush();
        }

        [TestMethod]
        public void StoredAndLoaded()
        {
            var column2 = new ColumnBase<double>("t1", "c1", registry.DoubleTraits);
            var whole1 = column1.Get(null);
            var whole2 = column2.Get(null);

            CollectionAssert.AreEqual(whole1.All(), whole2.All());
        }
    }
}
