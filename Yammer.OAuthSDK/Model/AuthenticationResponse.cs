using System.Runtime.Serialization;

namespace Yammer.OAuthSDK.Model
{
    /// <summary>
    /// The root object that deserializes from an Yammer OAuth API call response.
    /// </summary>
    [DataContract]
    public class AuthenticationResponse
    {
        /// <summary>
        /// The object that contains the actual token.
        /// </summary>
        [DataMember(Name = "access_token")]
        public AccessToken AccessToken { get; set; }

        /// <summary>
        /// This is documented as "error" but actually called "response".
        /// The object that contains information about an erronous API call.
        /// </summary>
        [DataMember(Name = "response")]
        public OAuthError OAuthError { get; set; }
    }
}
