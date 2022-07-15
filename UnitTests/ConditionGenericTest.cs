using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class ConditionGenericTest
    {
        [TestMethod]
        [DataRow(2, ">", "1")]
        [DataRow(3, "<", "5")]
        [DataRow(2, ">=", "1")]
        [DataRow(3, "<=", "5")]
        [DataRow(42, "=", "42")]
        [DataRow(5, "<>", "7")]
        public void ConditionTrue(int val, string op, string origin)
        {
            var condition = Condition<int>.Transform(op, origin);
            Assert.IsTrue(condition.Satisfies(val));
        }

        [TestMethod]
        [DataRow(2, "<", "1")]
        [DataRow(3, ">", "5")]
        [DataRow(2, "<=", "1")]
        [DataRow(3, ">=", "5")]
        [DataRow(42, "<>", "42")]
        [DataRow(5, "=", "7")]
        public void ConditionFalse(int val, string op, string origin)
        {
            var condition = Condition<int>.Transform(op, origin);
            Assert.IsFalse(condition.Satisfies(val));
        }
    }
}
