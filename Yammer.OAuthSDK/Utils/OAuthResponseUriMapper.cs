using System;
using System.Net;
using System.Windows.Navigation;

namespace Yammer.OAuthSDK.Utils
{
    /// <summary>
    /// Converts a uniform resource identifier (URI) into a new URI to be redirected to based on the OAuth parameters received.
    /// </summary>
    public class OAuthResponseUriMapper : UriMapperBase
    {
        string redirectUri;

        /// <summary>
        /// We have a dependency on a redirectUri for this mapper to work.
        /// </summary>
        /// <param name="redirectUri">The App should provide the URL that will handle the result of authorization.</param>
        public OAuthResponseUriMapper(string redirectUri)
        {
            this.redirectUri = redirectUri;
        }

        /// <summary>
        /// Converts a requested uniform resource identifier (URI) to a new URI.
        /// </summary>
        /// <param name="uri">The original URI value to be mapped to a new URI.</param>
        /// <returns>A URI to use for the request instead of the value in the uri parameter.</returns>
        public override Uri MapUri(Uri uri)
        {
            string decodedUri = HttpUtility.UrlDecode(uri.ToString());

            // URI association launch for this app.
            if (decodedUri.Contains(redirectUri))
            {
                // Extract the Uri query params (Uri looks like /Protocol?encodedLaunchUri=myappscheme://something.com/?code=p5EovkhKGrAAASmhNoUMQ)
                int redirectUriIndex = decodedUri.IndexOf(redirectUri);
                string redirectParams = new Uri (decodedUri.Substring(redirectUriIndex)).Query;
                // Map the OAuth response to the app page
                return new Uri("/MainPage.xaml" + redirectParams, UriKind.Relative);
            }

            // Otherwise perform normal launch.
            return uri;
        }
    }
}
