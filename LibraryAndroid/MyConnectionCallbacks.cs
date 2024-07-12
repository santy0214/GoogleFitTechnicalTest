using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryAndroid
{
    [Obsolete]
    public class MyConnectionCallbacks : Java.Lang.Object, GoogleApiClient.IConnectionCallbacks
    {
        public async void OnConnected(Bundle connectionHint)
        {
            // GoogleApiClient se ha conectado
            await GoogleFitAuth.HandleSignInResultAsync(GoogleFitAuth.task);
        }

        public void OnConnectionSuspended(int cause)
        {
            GoogleFitAuth.googleApiClient.Connect(); // Intentar reconectar
            // Manejar la suspensión de la conexión
        }


    }

}