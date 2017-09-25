using System;
using System.Collections.Generic;
using System.IO;

namespace FreeProxyListLoader.Models.Serialization
{
    class TextSimpleSerializer : ISerializer
    {
        #region ISerializer Implementation

        public void Serialize<T>(Stream stream, T serializingObject)
        {
            using (var sw = new StreamWriter(stream))
            {
                var proxies = serializingObject as List<FreeProxy> ?? 
                    throw new ArgumentException("FreeProxy collection expected");

                foreach (var proxy in proxies)
                    sw.WriteLine(proxy);
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
