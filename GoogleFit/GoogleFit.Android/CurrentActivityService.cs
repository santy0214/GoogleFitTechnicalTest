using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GoogleFit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace GoogleFit.Droid
{
    public class CurrentActivityService : ICurrentActivity
    {

        private Activity platform;
        private Intent _authenticator;
        private Context _context;

        public void GetAuthenticator()
        {
            _context.StartActivity(_authenticator);
        }

        public object GetCurrentActivity()
        {
            return platform; // Acceso a la actividad actual
        }

        public void NewConnectGoogleFit()
        {
            LibraryAndroid.GoogleFitAuth.Initialize(_context, platform);
        }

        public void SetCurrentActivity(object actividad, object authenticator)
        {
            _context = (Activity)actividad;
            _authenticator = (Intent)authenticator;
        }

        public void SetCurrentActivityGoogle(object context, object activity)
        {
            _context = (Context)context;
            platform = (Activity)activity;
        }
    }
}