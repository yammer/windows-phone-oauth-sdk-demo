Yammer API Windows Phone 8 OAuth SDK
========================

SDK tools and a demo for Windows Phone users to consume the Yammer API via Oauth.

Introduction
------------
The Yammer API opens Yammer networks to third-party application developers.  This sample application provides Windows Phone developers with the code necessary to integrate Yammer functionality into mobile apps. It demonstrates a step-by-step process that does the following:

1. Allows users to login to the Yammer network using the IE browser
2. Obtains an authToken and stores it securely to the Isolated Storage
3. Uses that authToken to make all subsequent calls to the Yammer API

In order to provide this functionality, some setup must be done.

App Setup
---------

The solution consists of two projects: 

**Yammer.OAuthSDK** is a Windows Phone 8 class library project that contains helper methods to help you handle the OAuth login and authorization process, as well as giving a headstart of how to call an API once we have a token.

**OAuthWPDemo** is an actual Windows Phone 8 application that demonstrates how to setup and use these helper classes in an App.

**Steps to setup your own working app:**

**Step 1)** Register your Yammer application here: https://www.yammer.com/client_applications

**Step 2)** As part of the application setup in step 1, set the Redirect URI to a custom URI scheme.  This must be unique to your WP8 app.  Here's an example: **wp8oauthdemo://something**
<br/>Make sure the scheme name (in this case "wp8oauthdemo") is unique to your company and WP8 app.

**Step 3)** You will need to create a instance of **Yammer.OAuthSDK.Model.OAuthClientInfo** with your app's values found in [Yammer client application](https://www.yammer.com/client_applications). The best place to do this is on the Application resource dictionary in your App.xaml file e.g.

```XML
<!--Application Resources-->
<Application.Resources>
  <!- ... ->
  <model:OAuthClientInfo xmlns:model="clr-namespace:Yammer.OAuthSDK.Model;assembly=Yammer.OAuthSDK" x:Key="MyOAuthClientInfo"
      ClientId="XXXXXXXXXXXXXX" 
      ClientSecret="YYYYYYYYYYYYYYYYYYYY" 
      RedirectUri="wp8oauthdemo://something" />
</Application.Resources>
```

Note the creation of a Property getter to facilitate access to this object Application-wide in **App.xaml.cs**:

```csharp
/// <summary>
/// Easy access to the OAuth Client information defined in the App resource dictionary.
/// </summary>
public OAuthClientInfo MyOAuthClientInfo
{
    get
    {
        return Resources["MyOAuthClientInfo"] as OAuthClientInfo;
    }
}
```

**Step 4)** During the login process, users will be directed to the mobile IE web browser.  In order for the browser to be able to switch back to your WP app, the custom URL Scheme from step 2 must be registered in the WP application.  Here's how you do that:
<br/><br/>
To register for a URI association, you must edit **WMAppManifest.xml** using the XML (Text) Editor. In Solution Explorer, expand the Properties folder and right-click the WMAppManifest.xml file, and then click Open With. In the Open With window, select XML(Text) Editor, and then click OK.
<br/><br/>
In the Extensions element of the app manifest file, a URI association is specified with a Protocol element (note that the Extensions element must immediately follow the Tokens element). Your Extensions element should look like this:

```XML
<Extensions>
  <Protocol Name="wp8oauthdemo" NavUriFragment="encodedLaunchUri=%s" TaskID="_default" />
</Extensions>
```

Also make sure to override the default URI-mapper class with our OAuth URI handler in the InitializePhoneApplication() method in **App.xaml.cs**:

```csharp
// Override the default URI-mapper class with our OAuth URI handler.
RootFrame.UriMapper = new OAuthResponseUriMapper(MyOAuthClientInfo.RedirectUri);
```

***
**That's it.**  <br/>
After that you should be ready to go. Take a look at how the method 

```csharp
OAuthUtils.LaunchSignIn(string clientId, string redirectUri)
```

is implemented and used in MainPage.xaml.cs::btnSignInWithYammer_Click. That's the method that launches the IE browser and lets the user both authenticate, and Authorize the app. 
<br/>
After that take a look at

```csharp
OAuthUtils.HandleApprove(string clientId,
            string clientSecret,
            string code, 
            string state,
            Action onSuccess, 
            Action onCSRF = null, 
            Action<AuthenticationResponse> onErrorResponse = null, 
            Action<Exception> onException = null)
```

which is used in MainPage.xaml.cs::OnNavigatedTo after a user has been redirected back from the Login/Authorization page.
<br />
Finally, note the use of 

```csharp
OAuthUtils.GetJsonFromApi(Uri endpoint, 
            Action<string> onSuccess, 
            Action<AuthenticationResponse> onErrorResponse = null, 
            Action<Exception> onException = null)
```

in MainPage.xaml.cs::btnCallFollowingApi_Click to handle the simple dump of a json result of an API call after the user has been succesfully authenticated.
rovide the same "re-entry" functionality, you need to add this app delegate method to your WP app delegate.

**Note:** Once the server sees that a user has clicked the Allow button, future login requests do not display the page with the Allow button.  This is a one time occurance for each unique user/yammer-app combination.  Subsequent login attempts will return directly to the WP8 app without the Allow page. You can manually Revoke Access to your application by going to [your apps listing page](https://www.staging.yammer.com/account/applications).
