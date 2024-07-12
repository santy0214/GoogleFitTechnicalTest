using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleFit.Interfaces
{
    public interface ICurrentActivity
    {
        object GetCurrentActivity();
        void GetAuthenticator();
        void NewConnectGoogleFit();
        void SetCurrentActivity(Object actividad, Object authenticator);
        void SetCurrentActivityGoogle(Object context, Object activity);
    }
}
