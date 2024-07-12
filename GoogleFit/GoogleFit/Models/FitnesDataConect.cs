using Google.Apis.Auth.OAuth2;
using Google.Apis.Fitness.v1;
using Google.Apis.Fitness.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Google.Apis.Auth.OAuth2.Flows;
using Xamarin.Auth;
using Xamarin.Essentials;
using System.Text;
using static Google.Apis.Fitness.v1.FitnessService;
using GoogleFit.Services;

namespace GoogleFit.Models
{
    public class FitnesDataConect : IFitnessManager
    {

        public void AuthorizeOauhtGoogleFit(OAuth2Authenticator authenticator)
        {
            authenticator.DoNotEscapeScope = false;

            authenticator.AllowCancel = true;
            authenticator.ShowErrors = false;
            authenticator.ClearCookiesBeforeLogin = true;

            authenticator.Completed += async (sender, args) =>
            {
                if (args.IsAuthenticated)
                {
                    string accessToken = args.Account.Properties["access_token"];
                    string refreshToken = args.Account.Properties["refresh_token"];

                    Globales.Token = accessToken;
                    Globales.refreshToken = refreshToken;
                    Preferences.Set("GoogleFitAccessToken", accessToken);
                    Preferences.Set("GoogleFitRefreshToken", refreshToken);

                    ShowAlert("Information", "The connection with Google Fit was successful", "Ok");
                }
            };


            authenticator.Error +=
                (s, ea) =>
                {
                    ShowAlert("Error", ea.Message, "Ok");
      
                    return;
                };

        }


        async void ShowAlert(string title, string message, string button)
        {
            await App.Current.MainPage.DisplayAlert(title, message, button);
        }


        public OAuth2Authenticator Initialize(object activity)
        {
            OAuth2Authenticator authenticator;
            string url = $"{Globales.authorizeURL}?client_id={Uri.EscapeDataString(Globales.clientId)}&access_type=offline&prompt=consent&include_granted_scopes=true&";

            authenticator = new OAuth2Authenticator(
                clientId: Globales.clientId,
                clientSecret: null,
                scope: Globales.scope,
                authorizeUrl: new Uri(url),
                redirectUrl: new Uri(Globales.redirectUri),
                accessTokenUrl: new Uri(Globales.accessTokenURL),
                isUsingNativeUI: false);
            // Crear un PendingIntent con FLAG_IMMUTABLE


            return authenticator;

        }

        public void Inicialize()
        {
            

        }

        public void Inicialize(object activity)
        {
            throw new NotImplementedException();
        }

        public OAuth2Authenticator Initialize()
        {
            throw new NotImplementedException();
        }

        public void NewConnectGoogleFit()
        {
        }
    }
}
