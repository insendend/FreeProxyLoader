using System.IO;
using System.Runtime.Serialization.Json;

namespace FreeProxyListLoader.Models.Serialization
{
    public class JsonSimpleSerializer : ISerializer
    {
        #region ISerializer Implementation

        public void Serialize<T>(Stream stream, T serializingObject)
        {
            using (stream)
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(stream, serializingObject);
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T) serializer.ReadObject(stream);
        }

        #endregion
    }
}
