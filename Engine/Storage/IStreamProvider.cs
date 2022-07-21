using System.IO;

namespace Engine.Storage
{
    public interface IStreamProvider
    {
        Stream OpenRead(string name);
        Stream OpenReadWrite(string name);
    }
}
