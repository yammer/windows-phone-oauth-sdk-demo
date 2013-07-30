
namespace Yammer.OAuthSDK.Model
{
    /// <summary>
    /// Constants used to identify your app on the Yammer Platform
    /// These values should match the ones in https://www.yammer.com/client_applications
    /// </summary>
    public class OAuthClientInfo
    {
        /// <summary>
        /// Unique ID assigned by Yammer to identify your app
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Unique key used to authenticate your app to Yammer
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// The URL that will handle the result of authorization
        /// </summary>
        public string RedirectUri { get; set; }
    }
}
