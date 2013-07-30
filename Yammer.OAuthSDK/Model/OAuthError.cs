using System.Net;
using System.Runtime.Serialization;

namespace Yammer.OAuthSDK.Model
{
    /// <summary>
    /// Object used to deserialize an error response from an Yammer API call.
    /// </summary>
    [DataContract]
    public class OAuthError
    {
        /// <summary>
        /// Error type (doesn't seem to be used)
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        [DataMember(Name = "message")]
        public string Message { get; set; }

        /// <summary>
        /// Error code
        /// </summary>
        [DataMember(Name = "code")]
        public string Code { get; set; }

        /// <summary>
        /// Error stat
        /// </summary>
        [DataMember(Name = "stat")]
        public string Stat { get; set; }

        /// <summary>
        /// Used to store the HTTP status code of the response.
        /// Not used for serialization.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }

        /// <summary>
        /// Used to store the HTTP status description of the response.
        /// Not used for serialization.
        /// </summary>
        public string HttpStatusDescription { get; set; }

        /// <summary>
        /// Friendly text representation of the error object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("HTTP Response: {0} - {1}\ntype: {2}\nmessage: {3}\ncode: {4}\nstat: {5}", (int)HttpStatusCode, HttpStatusDescription, Type, Message, Code, Stat);
        }
    }
}
