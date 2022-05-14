using System.Collections.Generic;
using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class MsgPackTest
    {
        [TestMethod]
        public void MPackSerializeResultWithColumns()
        {
            var dColumn = new ResultColumnDouble("dc", new double[] { -0.001, 0, 0.002 });
            var iColumn = new ResultColumnInteger("ic", new int[] { 42, 333 });
            var sColumn = new ResultColumnString("sc", new string[] { "a string" });

            var columns = new List<ResultColumn> { dColumn, iColumn, sColumn };

            var result = new Result(columns);

            var packer = new Engine.Serialization.MPackResultPacker();

            var buffer = packer.PackResult(result);

            var outRes = packer.UnpackResult(buffer);

            Assert.IsNull(outRes.Error);
            Assert.AreEqual(result.Columns.Count, outRes.Columns.Count);
            Assert.IsInstanceOfType(result.Columns[0], typeof(ResultColumnDouble));
            Assert.IsInstanceOfType(result.Columns[1], typeof(ResultColumnInteger));
            Assert.IsInstanceOfType(result.Columns[2], typeof(ResultColumnString));
            Assert.AreEqual(result.Columns[0].Name, outRes.Columns[0].Name);
            Assert.AreEqual(result.Columns[1].Name, outRes.Columns[1].Name);
            Assert.AreEqual(result.Columns[2].Name, outRes.Columns[2].Name);
            Assert.AreEqual(result.Columns[0].Count, outRes.Columns[0].Count);
            Assert.AreEqual(result.Columns[1].Count, outRes.Columns[1].Count);
            Assert.AreEqual(result.Columns[2].Count, outRes.Columns[2].Count);
            Assert.AreEqual(dColumn[0], ((ResultColumnDouble)outRes.Columns[0])[0]);
            Assert.AreEqual(dColumn[1], ((ResultColumnDouble)outRes.Columns[0])[1]);
            Assert.AreEqual(dColumn[2], ((ResultColumnDouble)outRes.Columns[0])[2]);
            Assert.AreEqual(iColumn[0], ((ResultColumnInteger)outRes.Columns[1])[0]);
            Assert.AreEqual(iColumn[1], ((ResultColumnInteger)outRes.Columns[1])[1]);
            Assert.AreEqual(sColumn[0], ((ResultColumnString)outRes.Columns[2])[0]);
        }
    }
}