using Engine.Serialization;

namespace Engine.Storage
{
    public class DoublePage : PageStorage<double>
    {
        public DoublePage(IStreamProvider streams, CacheHost cacheHost)
            : base(streams, cacheHost)
        {
        }

        protected override double UnpackValue(byte[] buffer, ref int offset)
        {
            return BytePacker.UnpackDouble(buffer, ref offset);
        }

        protected override void PackValue(byte[] buffer, double value, ref int offset)
        {
            BytePacker.PackDouble(buffer, value, ref offset);
        }

        protected override int CalcMaxPairSize(double value)
        {
            return sizeof(long) + sizeof(double);
        }
    }
}