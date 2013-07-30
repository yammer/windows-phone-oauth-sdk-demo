using System.Runtime.Serialization;

namespace Yammer.OAuthSDK.Model
{
    /// <summary>
    ///  The object that contains the actual access token.
    /// </summary>
    [DataContract]
    public class AccessToken
    {
        /// <summary>
        /// The access token string.
        /// </summary>
        [DataMember(Name = "token")]
        public string Token { get; set; }
    }
}
