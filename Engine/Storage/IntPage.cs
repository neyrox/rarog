using Engine.Serialization;

namespace Engine.Storage
{
    public class IntPage : PageStorage<int>
    {
        public IntPage(IStreamProvider streams, CacheHost cacheHost)
            : base(streams, cacheHost)
        {
        }

        protected override int UnpackValue(byte[] buffer, ref int offset)
        {
            return BytePacker.UnpackSInt32(buffer, ref offset);
        }

        protected override void PackValue(byte[] buffer, int value, ref int offset)
        {
            BytePacker.PackSInt32(buffer, value, ref offset);
        }

        protected override int CalcMaxPairSize(int value)
        {
            return sizeof(long) + sizeof(int);
        }
    }
}
