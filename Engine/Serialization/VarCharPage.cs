using System.Text;
using Engine.Serialization;

namespace Engine.Storage
{
    public class VarCharPage : PageBase<string>
    {
        public static VarCharPage Instance => new VarCharPage();

        protected override string UnpackValue(byte[] buffer, ref int offset)
        {
            return BytePacker.UnpackString16(buffer, ref offset);
        }

        protected override void PackValue(byte[] buffer, string value, ref int offset)
        {
            BytePacker.PackString16(buffer, value, ref offset);
        }

        protected override int CalcMaxPairSize(string value)
        {
            var maxValueLength = value?.Length * 4 ?? 0;
            // Leave 64Kb space for updates to longer strings
            return sizeof(long) + sizeof(ushort) + maxValueLength + 65536;
            //Encoding.UTF8.GetByteCount(value);
        }
    }
}
