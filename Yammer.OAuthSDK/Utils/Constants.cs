
namespace Yammer.OAuthSDK.Utils
{
    public static class Constants
    {
        /// <summary>
        /// Constants that point to the Yammer API endpoints
        /// </summary>
        public static class ApiEndpoints
        {
            /// <summary>
            /// User Authentication endpoint
            /// </summary>
            public const string OAuthUserAuthentication = @"https://www.yammer.com/dialog/oauth?client_id={0}&redirect_uri={1}&state={2}";

            /// <summary>
            /// App Authentication endpoint
            /// </summary>
            public const string OAuthAppAuthentication = @"https://www.yammer.com/oauth2/access_token.json?client_id={0}&client_secret={1}&code={2}";

            /// <summary>
            /// Following API endpoint
            /// </summary>
            public const string Following = @"https://www.yammer.com/api/v1/messages/following.json";

            /// <summary>
            /// A link to the list of your registered Yammer apps
            /// </summary>
            public const string MyApps = @"https://www.yammer.com/account/applications";
        }

        /// <summary>
        /// Contants that are used as the URL parameters for the API calls and responses
        /// </summary>
        public static class OAuthParameters
        {
            /// <summary>
            /// Authorization code. If the user clicked "Allow".
            /// </summary>
            public const string Code = "code";

            /// <summary>
            /// The error type, example: “access_denied”.
            /// </summary>
            public const string Error = "error";

            /// <summary>
            /// A more fully formed human readable error, example: "The user denied your request".
            /// </summary>
            public const string ErrorDescription = "error_description";

            /// <summary>
            /// An optional unique string (nonce) to be used as a anti-CSRF token
            /// </summary>
            public const string State = "state";
        }
    }
}
