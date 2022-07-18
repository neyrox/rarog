using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Engine.Storage;

namespace UnitTests
{
    [TestClass]
    public class IntColumnStoreTest
    {
        private ColumnInteger column1;
        private IStorage storage;

        [TestInitialize]
        public void Setup()
        {
            storage = new MemoryStorage();

            column1 = new ColumnInteger("t1", "c1");
            column1.Insert(0, "100", storage);
            column1.Insert(1, "200", storage);
            column1.Insert(2, "300", storage);
            column1.Insert(5, "600", storage);
            column1.Insert(7, "800", storage);
        }

        [TestMethod]
        public void AllSelected()
        {
            var column2 = new ColumnInteger("t1", "c1");
            var whole1 = column1.Get(null, storage);
            var whole2 = column2.Get(null, storage);

            CollectionAssert.AreEqual(whole1.All(), whole2.All());
        }
    }
}
