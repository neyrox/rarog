using Engine.Serialization;

namespace Engine.Storage
{
    public class DoublePage : PageBase<double>
    {
        public static DoublePage Instance => new DoublePage();

        protected override double UnpackValue(byte[] buffer, ref int offset)
        {
            return BytePacker.UnpackDouble(buffer, ref offset);
        }

        protected override void PackValue(byte[] buffer, double value, ref int offset)
        {
            BytePacker.PackDouble(buffer, value, ref offset);
        }
    }
}