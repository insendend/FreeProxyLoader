using System.IO;
using System.Xml.Serialization;

namespace FreeProxyListLoader.Models.Serialization
{
    public class XmlSimpleSerializer : ISerializer
    {
        #region ISerializer Implementation

        public void Serialize<T>(Stream stream, T serializingObject)
        {
            using (stream)
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, serializingObject);
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T) serializer.Deserialize(stream);
        }

        #endregion
    }
}
