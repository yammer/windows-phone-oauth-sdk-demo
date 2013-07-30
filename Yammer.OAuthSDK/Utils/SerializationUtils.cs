using System.IO;
using System.Runtime.Serialization.Json;

namespace Yammer.OAuthSDK.Utils
{
    /// <summary>
    /// Utils class to handle serialization operations.
    /// </summary>
    public class SerializationUtils
    {
        /// <summary>
        /// Deserializes a stream the contains a json text into an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized into.</typeparam>
        /// <param name="stream">The strea that contains the json text representation of the object.</param>
        /// <returns>A deserialized object.</returns>
        public static T DeserializeJson<T>(Stream stream) where T : class 
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
            return jsonSerializer.ReadObject(stream) as T;
        }
    }
}
