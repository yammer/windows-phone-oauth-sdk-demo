using Microsoft.Phone.Tasks;
using System;
using System.Net;
using Yammer.OAuthSDK.Model;

namespace Yammer.OAuthSDK.Utils
{
    /// <summary>
    /// Utils class to handle Yammer OAuth API operations.
    /// </summary>
    public static class OAuthUtils
    {
        private const string tokenFilePath = "tokenFilePath";

        private const string nonceFilePath = "nonceFilePath";

        /// <summary>
        /// Encrypts and decrypts an access token from IsolatedStorage
        /// </summary>
        public static string AccessToken
        {
            get
            {
                return CryptoUtils.DecryptStored(tokenFilePath);
            }
            set
            {
                CryptoUtils.EncryptAndStore(value, tokenFilePath);
            }
        }

        /// <summary>
        /// Launches Internet Explorer using a WebBrowserTask to redirect the user to the proper 
        /// User Authentication endpoint using the parameters from the Constants class.
        /// </summary>
        public static void LaunchSignIn(string clientId, string redirectUri)
        {
            var ieTask = new WebBrowserTask();
            // need to generate and store this nonce to identify the request is ours when it comes back
            string nonce = CryptoUtils.GenerateUrlFriendlyNonce();
            StorageUtils.WriteToIsolatedStorage(nonce, nonceFilePath);
            string url = string.Format(Constants.ApiEndpoints.OAuthUserAuthentication, clientId, redirectUri, nonce);
            ieTask.Uri = new Uri(url, UriKind.Absolute);
            ieTask.Show();
        }

        /// <summary>
        /// Handles an OAuth approve response asynchronously.
        /// </summary>
        /// <param name="code">The 'code' value obtained back from Yammer on the RedirectUri callback.</param>
        /// <param name="state">The optional 'state' value used to mitigate CSRF attacks.</param>
        /// <param name="onSuccess">Action to be executed if aprroval is successful.</param>
        /// <param name="onCSRF">Action to be executed if an CSRF attack was detected.</param>
        /// <param name="onErrorResponse">Action to be executed if we got an error response from Yammer.</param>
        /// <param name="onException">Action to be executed if there is an unexpected exception.</param>
        public static void HandleApprove(string clientId,
            string clientSecret,
            string code, 
            string state,
            Action onSuccess, 
            Action onCSRF = null, 
            Action<AuthenticationResponse> onErrorResponse = null, 
            Action<Exception> onException = null)
        {
            // we get the stored nonce from the Isolated Storage to verify it against the one we get back from Yammer
            string nonce = StorageUtils.ReadStringFromIsolatedStorage(nonceFilePath);
            if (state != nonce)
            {
                // might be a CSRF attack, so we discard the request
                if (onCSRF != null)
                {
                    onCSRF();
                }
                return;
            }
            string url = string.Format(Constants.ApiEndpoints.OAuthAppAuthentication, clientId, clientSecret, code);
            var appAuthUri = new Uri(url, UriKind.Absolute);

            var webclient = new WebClient();

            OpenReadCompletedEventHandler handler = null;
            handler = (s, e) =>
            {
                webclient.OpenReadCompleted -= handler;
                if (e.Error == null)
                {
                    // the token should have been sent back in json format, we use serialization to extract it
                    AuthenticationResponse oauthResponse = SerializationUtils.DeserializeJson<AuthenticationResponse>(e.Result);
                    AccessToken = oauthResponse.AccessToken.Token;
                    onSuccess();
                }
                else
                {
                    HandleExceptions(e.Error, onErrorResponse, onException);
                }
            };

            webclient.OpenReadCompleted += handler;
            // make the actual call to the Yammer OAuth App Authentication endpoint to get our token back
            webclient.OpenReadAsync(appAuthUri);
        }

        /// <summary>
        /// Handles an API call asynchronously.
        /// </summary>
        /// <param name="endpoint">An API URI endpoint that doesn't require any extra parameters.</param>
        /// <param name="onSuccess">Action to be executed is call is successful.</param>
        /// <param name="onErrorResponse">Action to be executed if we got an error response from Yammer.</param>
        /// <param name="onException">Action to be executed if there is an unexpected exception.</param>
        public static void GetJsonFromApi(Uri endpoint, 
            Action<string> onSuccess, 
            Action<AuthenticationResponse> onErrorResponse = null, 
            Action<Exception> onException = null)
        {
            if (endpoint == null || onSuccess == null)
            {
                throw new ArgumentNullException();
            }

            var webclient = new WebClient();
            // We shouldn't use the url query paramters to send the token, we should use the header to send it more securely instead
            webclient.Headers[HttpRequestHeader.Authorization] = "Bearer " + AccessToken;

            DownloadStringCompletedEventHandler handler = null;
            handler = (s, e) =>
            {
                webclient.DownloadStringCompleted -= handler;
                if (e.Error == null)
                {
                    var result = e.Result;
                    // We just pass the raw text data response to the callback
                    onSuccess(result);
                }
                else
                {
                    HandleExceptions(e.Error, onErrorResponse, onException);
                }
            };

            webclient.DownloadStringCompleted += handler;
            webclient.DownloadStringAsync(endpoint);
        }

        /// <summary>
        ///  Deleted the stored token from Isolated Storage
        /// </summary>
        public static void DeleteStoredToken()
        {
            StorageUtils.DeleteFromIsolatedStorage(tokenFilePath);
        }

        /// <summary>
        /// Extracts error response from WebExceptions (http error code responses)
        /// </summary>
        /// <param name="ex">The exception to extract the info from.</param>
        /// <param name="onErrorResponse">Action to be executed if we got an error response from Yammer.</param>
        /// <param name="onException">Action to be executed if there is an unexpected exception.</param>
        private static void HandleExceptions(Exception ex, Action<AuthenticationResponse> onErrorResponse, Action<Exception> onException)
        {
            if (ex.GetType().Name == "WebException" && onErrorResponse != null)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)((WebException)ex).Response;
                // the http response should include extra error details as a json, we use serialization to extract it
                AuthenticationResponse apiResponse = SerializationUtils.DeserializeJson<AuthenticationResponse>(httpResponse.GetResponseStream());
                // we also extract extra errror details from the HTTP status in the exception
                apiResponse.OAuthError.HttpStatusCode = httpResponse.StatusCode;
                apiResponse.OAuthError.HttpStatusDescription = httpResponse.StatusDescription;
                // we pass an object with all this error data to the callback
                onErrorResponse(apiResponse);
            }
            else if (onException != null)
            {
                onException(ex);
            }
        }
    }
}
