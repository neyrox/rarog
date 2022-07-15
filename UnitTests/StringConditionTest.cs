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
            var condition = new ConditionLess<string>("bbb");
            Assert.IsTrue(condition.Satisfies("aaa"));
            Assert.IsFalse(condition.Satisfies("bbb"));
            Assert.IsFalse(condition.Satisfies("ccc"));
        }

        [TestMethod]
        public void StringLessOrEqualToAString()
        {
            var condition = new ConditionLessOrEqual<string>("bbb");
            Assert.IsTrue(condition.Satisfies("aaa"));
            Assert.IsTrue(condition.Satisfies("bbb"));
            Assert.IsFalse(condition.Satisfies("ccc"));
        }

        [TestMethod]
        public void StringGreaterThenAString()
        {
            var condition = new ConditionGreater<string>("bbb");
            Assert.IsTrue(condition.Satisfies("ccc"));
            Assert.IsFalse(condition.Satisfies("bbb"));
            Assert.IsFalse(condition.Satisfies("aaa"));
        }

        [TestMethod]
        public void StringGreaterOrEqualToAString()
        {
            var condition = new ConditionGreaterOrEqual<string>("bbb");
            Assert.IsTrue(condition.Satisfies("ccc"));
            Assert.IsTrue(condition.Satisfies("bbb"));
            Assert.IsFalse(condition.Satisfies("aaa"));
        }
    }
}
