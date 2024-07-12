using Android.App;
using Android.Content;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Util;

namespace LibraryAndroid
{
    [Obsolete]
    public class MyOnConnectionFailedListener : Java.Lang.Object, GoogleApiClient.IOnConnectionFailedListener
    {
        private const int RC_SIGN_IN = 9001;



        public void OnConnectionFailed(ConnectionResult result)
        {
            if (result.HasResolution)
            {
                try
                {
                    // Inicia la actividad para resolver el error (es decir, iniciar sesión)
                    result.StartResolutionForResult(GoogleFitAuth._activity, RC_SIGN_IN);
                }
                catch (IntentSender.SendIntentException e)
                {
                    // Error al iniciar la resolución, intentar conectar de nuevo
                    GoogleFitAuth.googleApiClient.Connect();
                }
            }
            else
            {
                // Error sin resolución, manejar de acuerdo a tus necesidades
                Log.Error("GoogleApiClient", "Connection failed with result: " + result);
            }
            // Maneja la falla de conexión aquí
            Console.WriteLine("GoogleApiClient connection failed: " + result.ErrorMessage);
        }
    }
}