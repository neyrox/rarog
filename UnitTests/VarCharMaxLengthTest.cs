using System.Collections.Generic;
using Engine;
using Engine.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class VarCharMaxLengthTest
    {
        private IStorage storage;
        private Registry registry;
        private ColumnBase<string> column1;

        [TestInitialize]
        public void Setup()
        {
            storage = new MemoryStorage();
            registry = new Registry(storage);

            column1 = new ColumnBase<string>("t1", "c1", registry.StrTraits, 3);
        }

        [TestMethod]
        public void TestFitNotClamped()
        {
            column1.Insert(0, "aaa");

            var row = column1.Get(new List<long> {0});
            Assert.AreEqual("aaa", row.Get(0));
        }
        
        [TestMethod]
        public void TestNotFitClamped()
        {
            column1.Insert(0, "fatty");

            var row = column1.Get(new List<long> {0});
            Assert.AreEqual("fat", row.Get(0));
        }
    }
}