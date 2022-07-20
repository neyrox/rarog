using Engine.Serialization;

namespace Engine.Storage
{
    public class BigIntPage : PageStorage<long>
    {
        public BigIntPage(IStreamProvider streams, CacheHost cacheHost)
            : base(streams, cacheHost)
        {
        }

        protected override long UnpackValue(byte[] buffer, ref int offset)
        {
            return BytePacker.UnpackSInt64(buffer, ref offset);
        }

        protected override void PackValue(byte[] buffer, long value, ref int offset)
        {
            BytePacker.PackSInt64(buffer, value, ref offset);
        }

        protected override int CalcMaxPairSize(long value)
        {
            return sizeof(long) + sizeof(long);
        }
    }
}
