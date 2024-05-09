using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.Util.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace GoogleFit.Droid
{


    [Activity(Label = "ViewRedirectUrlInterceptor", NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [
    IntentFilter
    (
        actions: new[] { Intent.ActionView },
        Categories = new[]
                {
                    Intent.CategoryDefault,
                    Intent.CategoryBrowsable
                },
        DataSchemes = new[]
                {
                    "com.companyname.googlefit",
                },
        DataPath = "/oauth2redirect"
    )
]
    public class ViewRedirectUrlInterceptor : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CustomTabsConfiguration.CustomTabsClosingMessage = null;
            global::Android.Net.Uri uri_android = Intent.Data;
            Uri uri_netfx = new Uri(uri_android.ToString());
            MainActivity.authenticator.OnPageLoading(uri_netfx);
            var intent = new Intent(this, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(intent);

            Finish();

        }
    }
}