using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Auth;
using Xamarin.Essentials;
using GoogleFit.Models;
using GoogleFit.Services;
using Xamarin.Forms;

namespace GoogleFit.Droid
{
    [Activity(Label = "GoogleFit", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private  GetFitnessInformation _stepReader;
        private IFitnessManager fitnessManager;

        public static OAuth2Authenticator authenticator { get;  set; }
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new App());

            Globales.Token = Preferences.Get("GoogleFitAccessToken", string.Empty);
            Globales.refreshToken = Preferences.Get("GoogleFitRefreshToken", string.Empty);

            if (!string.IsNullOrEmpty(Globales.Token))
            {
               _stepReader = new GetFitnessInformation(Globales.Token);
                await _stepReader.GetTotalStepsToday();
                if (!Globales.notConnect)
                {
                    ConnectGoogleFitOauth();
                }
            }
            else
            {
                ConnectGoogleFitOauth();
            }
        }

        private void ConnectGoogleFitOauth()
        {
            IFitnessManagerFactory fitnessManagerFactory = new FitnessManagerFactory();
            fitnessManager = fitnessManagerFactory.CreateFitnessManager();
            AuthorizeAndGetData();
        }


        private void AuthorizeAndGetData()
        {
            try
            {
                Activity currentActivity = Platform.CurrentActivity;
                authenticator = fitnessManager.Initialize();
                fitnessManager.AuthorizeOauhtGoogleFit(authenticator);
                StartActivity(authenticator.GetUI(this));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}