using Facebook;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App4
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignInPage : Page
    {
        public SignInPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private void ClickedSignInWithFacebook(object sender, RoutedEventArgs e)
        {
            WebView1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            var loginUrl = GetFacebookLoginUrl(AppId, ExtendedPermissions);
            WebView1.Navigate(loginUrl);
        }

        private const string AppId = "295464883888329";

        /// <summary>
        /// Extended permissions is a comma separated list of permissions to ask the user.
        /// </summary>
        /// <remarks>
        /// For extensive list of available extended permissions refer to 
        /// https://developers.facebook.com/docs/reference/api/permissions/
        /// </remarks>

        private const string ExtendedPermissions = "read_stream,read_friendlists,friends_about_me,friends_birthday,friends_checkins,friends_education_history,friends_events,friends_likes,friends_location,friends_relationships,friends_work_history";

        private readonly FacebookClient _fb = new FacebookClient();


        private Uri GetFacebookLoginUrl(string appId, string extendedPermissions)
        {
            dynamic parameters = new ExpandoObject();
            parameters.client_id = appId;
            parameters.redirect_uri = "https://www.facebook.com/connect/login_success.html";
            parameters.response_type = "token";
            parameters.display = "popup";

            // add the 'scope' parameter only if we have extendedPermissions.
            if (!string.IsNullOrWhiteSpace(extendedPermissions))
            {
                // A comma-delimited list of permissions
                parameters.scope = extendedPermissions;
            }

            return _fb.GetLoginUrl(parameters);
        }

        private void WebView1_LoadCompleted(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            FacebookOAuthResult oauthResult;

            if (!_fb.TryParseOAuthCallbackUrl(e.Uri, out oauthResult))
            {
                return;
            }
            
            if (oauthResult.IsSuccess)
            {
                var accessToken = oauthResult.AccessToken;
                LoginSucceded(accessToken);
            }
            else
            {
                // user cancelled
                WebView1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private async void LoginSucceded(string accessToken)
        {
            dynamic parameters = new ExpandoObject();
            parameters.access_token = accessToken;

            dynamic result = await _fb.GetTaskAsync("me", parameters);
            parameters = new ExpandoObject();
            parameters.id = result.id;
            parameters.access_token = accessToken;

            WebView1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            Frame.Navigate(typeof(MainPage), result);
        }
    }
}
