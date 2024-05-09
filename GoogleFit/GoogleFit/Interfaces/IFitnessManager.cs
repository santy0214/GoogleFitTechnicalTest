using Android.Content;
using Google.Apis.Fitness.v1.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace GoogleFit.Services
{
    public interface IFitnessManager
    {
        OAuth2Authenticator Initialize();

        void AuthorizeOauhtGoogleFit(OAuth2Authenticator authenticator);

    }
}
