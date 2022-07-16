using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class IntConditionTest
    {
        [TestMethod]
        public void IntEqualToInt()
        {
            var condition = new ConditionEqual<int>("5");
            Assert.IsFalse(condition.Satisfies(4));
            Assert.IsFalse(condition.Satisfies(-3));
            Assert.IsTrue(condition.Satisfies(5));
            Assert.IsFalse(condition.Satisfies(6));
            Assert.IsFalse(condition.Satisfies(-7));
        }

        [TestMethod]
        public void IntNotEqualToInt()
        {
            var condition = new ConditionNotEqual<int>("5");
            Assert.IsTrue(condition.Satisfies(-4));
            Assert.IsTrue(condition.Satisfies(3));
            Assert.IsFalse(condition.Satisfies(5));
            Assert.IsTrue(condition.Satisfies(-6));
            Assert.IsTrue(condition.Satisfies(7));
        }

        [TestMethod]
        public void IntLessThenInt()
        {
            var condition = new ConditionLess<int>("5");
            Assert.IsTrue(condition.Satisfies(4));
            Assert.IsFalse(condition.Satisfies(5));
            Assert.IsFalse(condition.Satisfies(6));
        }

        [TestMethod]
        public void IntLessOrEqualToInt()
        {
            var condition = new ConditionLessOrEqual<int>("5");
            Assert.IsTrue(condition.Satisfies(4));
            Assert.IsTrue(condition.Satisfies(5));
            Assert.IsFalse(condition.Satisfies(6));
        }

        [TestMethod]
        public void IntGreaterThenInt()
        {
            var condition = new ConditionGreater<int>("5");
            Assert.IsTrue(condition.Satisfies(6));
            Assert.IsFalse(condition.Satisfies(5));
            Assert.IsFalse(condition.Satisfies(4));
        }

        [TestMethod]
        public void IntGreaterOrEqualToInt()
        {
            var condition = new ConditionGreaterOrEqual<int>("5");
            Assert.IsTrue(condition.Satisfies(6));
            Assert.IsTrue(condition.Satisfies(5));
            Assert.IsFalse(condition.Satisfies(4));
        }
    }
}
