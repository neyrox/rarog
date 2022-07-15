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
    }
}