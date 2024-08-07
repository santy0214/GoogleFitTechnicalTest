﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleFit.Models
{
    public class Globales
    {
        //OAuth 2.0 setup for Google Fit

        public static string clientIdWeb = ""; 
        public static string clientId = "";
        public static string clientSecret = "";

        public static string GoogleFitApiUrl = "https://www.googleapis.com/fitness/v1/users/me/dataset:aggregate";

        public static string scope = "https://www.googleapis.com/auth/fitness.activity.read";
        public static string redirectUri = "com.companyname.googlefit:/oauth2redirect";

        public static string authorizeURL = "https://accounts.google.com/o/oauth2/auth";
        public static string accessTokenURL = "https://oauth2.googleapis.com/token";

        public static string pasos = "";
        public static string heartRate = "";
        public static string calories = "";
        public static string minutes = "";
        public static string responseType = "code";

        public static string Token;
        public static bool notConnect;
        public static string refreshToken;
    }
}
