using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine.Storage;

namespace UnitTests
{
    [TestClass]
    public class VarCharColumnStoreTest
    {
        private IStorage storage;
        private Registry registry;
        private ColumnBase<string> column1;

        [TestInitialize]
        public void Setup()
        {
            storage = new MemoryStorage();
            registry = new Registry(storage);

            column1 = new ColumnBase<string>("t1", "c1", registry.StrTraits);
            column1.Insert(0, "000");
            column1.Insert(1, "aaa");
            column1.Insert(2, "bbb");
            column1.Insert(5, "eee");
            column1.Insert(7, "ggg");

            registry.StrTraits.PageStorage.Flush();
        }

        [TestMethod]
        public void StoredAndLoaded()
        {
            var column2 = new ColumnBase<string>("t1", "c1", registry.StrTraits);
            var whole1 = column1.Get(null);
            var whole2 = column2.Get(null);

            CollectionAssert.AreEqual(whole1.All(), whole2.All());
        }
    }
}
