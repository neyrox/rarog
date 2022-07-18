namespace Engine.Storage
{
    public struct Page
    {
        public const int HeaderSize = 4 * 1024;  // block read size (4Kb)
        public const int Size = 512 * 1024;  // SSD block write size (512Kb)
    }
}
