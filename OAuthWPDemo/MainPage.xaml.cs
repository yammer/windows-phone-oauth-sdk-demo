using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using Yammer.OAuthSDK.Utils;

namespace OAuthWPDemo
{
    public partial class MainPage : PhoneApplicationPage
    {
        private string clientId;
        private string clientSecret;
        private string redirectUri;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // we extract these values from the App's Resource Dictionary config 
            clientId = ((App)App.Current).MyOAuthClientInfo.ClientId;
            clientSecret = ((App)App.Current).MyOAuthClientInfo.ClientSecret;
            redirectUri = ((App)App.Current).MyOAuthClientInfo.RedirectUri;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Check the arguments from the query string passed to the page.
            IDictionary<string, string> uriParams = NavigationContext.QueryString;

            // "Approve"
            if (uriParams.ContainsKey(Constants.OAuthParameters.Code) && uriParams.ContainsKey(Constants.OAuthParameters.State) && e.NavigationMode != NavigationMode.Back)
            {
                OAuthUtils.HandleApprove(
                    clientId, 
                    clientSecret,
                    uriParams[Constants.OAuthParameters.Code],
                    uriParams[Constants.OAuthParameters.State],
                    onSuccess: () =>
                    {
                        UpdateTokenMessage(true);
                    }, onCSRF: () =>
                    {
                        MessageBox.Show("Unknown 'state' parameter. Discarding the authentication attempt.", "Invalid redirect.", MessageBoxButton.OK);
                    }, onErrorResponse: errorResponse =>
                    {
                        Dispatcher.BeginInvoke(() => MessageBox.Show(errorResponse.OAuthError.ToString(), "Invalid operation", MessageBoxButton.OK));
                    }, onException: ex =>
                    {
                        Dispatcher.BeginInvoke(() => MessageBox.Show(ex.ToString(), "Unexpected exception!", MessageBoxButton.OK));
                    }
                );
            }
            // "Deny"
            else if (uriParams.ContainsKey(Constants.OAuthParameters.Error) && e.NavigationMode != NavigationMode.Back)
            {
                string error, errorDescription;
                error = uriParams[Constants.OAuthParameters.Error];
                uriParams.TryGetValue(Constants.OAuthParameters.ErrorDescription, out errorDescription);

                string msg = string.Format("error: {0}\nerror_description:{1}", error, errorDescription);
                MessageBox.Show(msg, "Error response is received.", MessageBoxButton.OK);

                OAuthUtils.DeleteStoredToken();

                UpdateTokenMessage(false);
            }

            // if token already exist
            if (!string.IsNullOrEmpty(OAuthUtils.AccessToken))
            {
                UpdateTokenMessage(true);
            }
        }
        
        private void btnSignInWithYammer_Click(object sender, RoutedEventArgs e)
        {
            OAuthUtils.LaunchSignIn(clientId, redirectUri);
        }

        private void btnCallFollowingApi_Click(object sender, RoutedEventArgs e)
        {
            // Call this API to test if the auth token works
            var followingApiEndpoint = new Uri(Constants.ApiEndpoints.Following, UriKind.Absolute);

            OAuthUtils.GetJsonFromApi(followingApiEndpoint,
                onSuccess: response =>
                {
                    // we just dump the unformated json string response into a textbox
                    Dispatcher.BeginInvoke(() => txtResponses.Text = response);
                }, onErrorResponse: errorResponse =>
                {
                    Dispatcher.BeginInvoke(() => 
                    {
                        MessageBox.Show(errorResponse.OAuthError.ToString(), "Invalid operation", MessageBoxButton.OK);
                        txtResponses.Text = string.Empty;
                    });
                }, onException: ex =>
                {
                    Dispatcher.BeginInvoke(() => 
                    {
                        MessageBox.Show(ex.ToString(), "Unexpected exception!", MessageBoxButton.OK);
                        txtResponses.Text = string.Empty;
                    });
                }
            );

            Dispatcher.BeginInvoke(() => txtResponses.Text = "Loading...");
        }

        private void btnRevokeAccess_Click(object sender, RoutedEventArgs e)
        {
            var ieTask = new WebBrowserTask();
            ieTask.Uri = new Uri(Constants.ApiEndpoints.MyApps, UriKind.Absolute);
            ieTask.Show();
        }

        private void btnDeleteToken_Click(object sender, RoutedEventArgs e)
        {
            OAuthUtils.DeleteStoredToken();
            UpdateTokenMessage(false);
        }

        /// <summary>
        /// Update the UI status of the token existance in IsolatedStorage.
        /// </summary>
        /// <param name="isTokenPresent">Whether the token is present in Isolated Sotrage or not.</param>
        private void UpdateTokenMessage(bool isTokenPresent)
        {
            Dispatcher.BeginInvoke(() => txbIsTokenPresent.Text = isTokenPresent ? txbIsTokenPresent.Text.Replace("No.", "Yes.") : txbIsTokenPresent.Text.Replace("Yes.", "No."));
        }
    }
}