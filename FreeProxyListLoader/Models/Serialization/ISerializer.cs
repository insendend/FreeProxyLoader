using System.IO;

namespace FreeProxyListLoader.Models.Serialization
{
    interface ISerializer
    {
        void Serialize<T>(Stream stream, T serializingObject);

        T Deserialize<T>(Stream stream);
    }
}
