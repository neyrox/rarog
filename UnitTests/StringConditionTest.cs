using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class StringConditionTest
    {
        [TestMethod]
        public void StringLessThenAString()
        {
            var condition = new ConditionStringLess("bbb");
            Assert.IsTrue(condition.Satisfies("aaa"));
            Assert.IsFalse(condition.Satisfies("bbb"));
            Assert.IsFalse(condition.Satisfies("ccc"));
        }

        [TestMethod]
        public void StringGreaterThenAString()
        {
            var condition = new ConditionStringGreater("bbb");
            Assert.IsTrue(condition.Satisfies("ccc"));
            Assert.IsFalse(condition.Satisfies("bbb"));
            Assert.IsFalse(condition.Satisfies("aaa"));
        }
    }
}
