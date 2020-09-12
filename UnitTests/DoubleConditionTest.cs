using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class DoubleConditionTest
    {
        [TestMethod]
        public void DoubleLessThenDouble()
        {
            var condition = new ConditionDoubleLess("5");
            Assert.IsTrue(condition.Satisfies(4));
            Assert.IsTrue(condition.Satisfies(4.999));
            Assert.IsFalse(condition.Satisfies(5));
            Assert.IsFalse(condition.Satisfies(5.001));
            Assert.IsFalse(condition.Satisfies(6));
        }

        [TestMethod]
        public void DoubleLessOrEqualToDouble()
        {
            var condition = new ConditionDoubleLessOrEqual("5");
            Assert.IsTrue(condition.Satisfies(4));
            Assert.IsTrue(condition.Satisfies(4.999));
            Assert.IsTrue(condition.Satisfies(5));
            Assert.IsFalse(condition.Satisfies(5.001));
            Assert.IsFalse(condition.Satisfies(6));
        }

        [TestMethod]
        public void DoubleGreaterThenDouble()
        {
            var condition = new ConditionDoubleGreater("5");
            Assert.IsTrue(condition.Satisfies(6));
            Assert.IsTrue(condition.Satisfies(5.001));
            Assert.IsFalse(condition.Satisfies(5));
            Assert.IsFalse(condition.Satisfies(4.999));
            Assert.IsFalse(condition.Satisfies(4));
        }

        [TestMethod]
        public void DoubleGreaterOrEqualToDouble()
        {
            var condition = new ConditionDoubleGreaterOrEqual("5");
            Assert.IsTrue(condition.Satisfies(6));
            Assert.IsTrue(condition.Satisfies(5.001));
            Assert.IsTrue(condition.Satisfies(5));
            Assert.IsFalse(condition.Satisfies(4.999));
            Assert.IsFalse(condition.Satisfies(4));
        }
    }
}
