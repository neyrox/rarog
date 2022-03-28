using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine;
using Engine.Serialization;

namespace UnitTests
{
    [TestClass]
    public class SerializationTest
    {
        [TestMethod]
        public void SerializeInt16()
        {
            byte[] buffer = new byte[100];
            int offset = 0;
            ushort val = 54321;
            BytePacker.PackUInt16(buffer, val, ref offset);
            Assert.AreEqual(2, offset);
            CollectionAssert.AreEqual(BitConverter.GetBytes(val), buffer.Take<byte>(2).ToArray());
            offset = 0;
            ulong outVal = BytePacker.UnpackUInt16(buffer, ref offset);
            Assert.AreEqual(2, offset);
            Assert.AreEqual(val, outVal);
        }

        [TestMethod]
        public void SerializeInt64()
        {
            byte[] buffer = new byte[100];
            int offset = 0;
            ulong val = 0xfedcba9876543210ul;
            BytePacker.PackUInt64(buffer, val, ref offset);
            Assert.AreEqual(8, offset);
            CollectionAssert.AreEqual(BitConverter.GetBytes(val), buffer.Take<byte>(8).ToArray());
            offset = 0;
            ulong outVal = BytePacker.UnpackUInt64(buffer, ref offset);
            Assert.AreEqual(8, offset);
            Assert.AreEqual(val, outVal);
        }

        [TestMethod]
        public void SerializeResultWithColumns()
        {
            var dColumn = new ResultColumnDouble(new double[] { -0.001, 0, 0.002 });
            var iColumn = new ResultColumnInteger(new int[] { 42, 333 });
            var sColumn = new ResultColumnString(new string[] { "a string" });

            var columns = new List<ResultColumn>() { dColumn, iColumn, sColumn };

            var result = new Result(columns);

            var packer = new Engine.Serialization.ResultPacker();
            
            var buffer = packer.PackResult(result, out var length);

            var outRes = packer.UnpackResult(buffer, 0);

            Assert.IsNull(outRes.Error);
            Assert.AreEqual(result.Columns.Count, outRes.Columns.Count);
            Assert.IsInstanceOfType(result.Columns[0], typeof(ResultColumnDouble));
            Assert.IsInstanceOfType(result.Columns[1], typeof(ResultColumnInteger));
            Assert.IsInstanceOfType(result.Columns[2], typeof(ResultColumnString));
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
