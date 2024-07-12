using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Auth;
using Xamarin.Essentials;
using GoogleFit.Models;
using GoogleFit.Services;
using Android.Content;
using AndroidX.Core.App;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using System.Threading.Tasks;
using Android.Gms.Tasks;
using Java.Interop;
using Android.Gms.Fitness.Request;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Util;
using Android.Gms.Common.Apis;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using Google.Apis.Fitness.v1;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Java.Util;
using GoogleFit.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Android.Runtime;
//using Com.Tactomotion.Utilidades.Funciones;



namespace GoogleFit.Droid
{
    [Activity(Label = "GoogleFit", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private GetFitnessInformation _stepReader;
        private IFitnessManager fitnessManager;

        private const int RC_SIGN_IN = 9001;
        private GoogleSignInClient mGoogleSignInClient;
        private GoogleSignInAccount mGoogleSignInAccount; // Almacena la cuenta de Google firmada

        public static OAuth2Authenticator authenticator { get; set; }


        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);



            InitializeDependencies();

            //// Configurar las opciones de inicio de sesión de Google
            //GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
            //    .RequestIdToken(Globales.clientIdWeb)
            //    .RequestEmail()
            //    .Build();

            //// Crear un cliente de inicio de sesión de Google
            //mGoogleSignInClient = GoogleSignIn.GetClient(this, gso);

            //// Iniciar el proceso de inicio de sesión
            //Android.Content.Intent signInIntent = mGoogleSignInClient.SignInIntent;
            //StartActivityForResult(signInIntent, RC_SIGN_IN);


            //GoogleHealthConnec();

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new App());

            //Globales.Token = Preferences.Get("GoogleFitAccessToken", string.Empty);
            //Globales.refreshToken = Preferences.Get("GoogleFitRefreshToken", string.Empty);

            //if (!string.IsNullOrEmpty(Globales.Token))
            //{
            //    _stepReader = new GetFitnessInformation(Globales.Token);
            //    await _stepReader.GetTotalStepsToday();
            //    if (!Globales.notConnect)
            //    {
            //        ConnectGoogleFitOauth();
            //    }
            //}
            //else
            //{
            //    ConnectGoogleFitOauth();
            //}

            //ConnectGoogleFitOauth();
        }



        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICurrentActivity, CurrentActivityService>();
        }

        public static IServiceProvider ServiceProvider { get; private set; }


        private void InitializeDependencies()
        {
            try
            {
                IFitnessManagerFactory fitnessManagerFactory = new FitnessManagerFactory();
                fitnessManager = fitnessManagerFactory.CreateFitnessManager();

                //authenticator = fitnessManager.Initialize();

                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                ServiceProvider = serviceCollection.BuildServiceProvider();
                ServiceProvider.GetService<ICurrentActivity>().SetCurrentActivityGoogle(this, this);

                var currentActivity = ServiceProvider.GetService<ICurrentActivity>().GetCurrentActivity() as Activity;
                DependencyServiceProvider.ServiceProvider = ServiceProvider;

                if (currentActivity == null)
                {
                    throw new InvalidOperationException("No se pudo obtener la actividad actual.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override async void OnActivityResult(int requestCode, [Android.Runtime.GeneratedEnum] Result resultCode, Android.Content.Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Resultado del inicio de sesión de Google
            if (requestCode == RC_SIGN_IN)
            {

                LibraryAndroid.GoogleFitAuth.task = GoogleSignIn.GetSignedInAccountFromIntent(data);

                LibraryAndroid.GoogleFitAuth.BuildGoogleApiClient();

                //Task<GoogleSignInAccount> signInTask = GetSignedInAccountFromIntentAsync(data);

                //HandleSignInResultViejo(signInTask);

                //GoogleSignIn.GetSignedInAccountFromIntent(data);


                //GoogleSignInAccount account = await GoogleSignIn.GetSignedInAccountFromIntentAsync(data);
                //HandleSignInResultAsync(account);

                //var result = await GoogleSignIn.GetSignedInAccountFromIntentAsync(data);


                // ReadDailyStepCountAsync();
                ////HandleSignInResult(null);
            }


        }






        #region "Otro Codigo"



        private Task<GoogleSignInAccount> GetSignedInAccountFromIntentAsync(Android.Content.Intent data)
        {
            TaskCompletionSource<GoogleSignInAccount> tcs = new TaskCompletionSource<GoogleSignInAccount>();

            System.Threading.Tasks.Task.Run(() =>
            {
                GoogleSignIn.GetSignedInAccountFromIntent(data)
                    .AddOnSuccessListener(new SignInSuccessListener(tcs))
                    .AddOnFailureListener(new SignInFailureListener(tcs));
            });

            return tcs.Task;
        }

        private class SignInSuccessListener : Java.Lang.Object, IOnSuccessListener
        {
            private TaskCompletionSource<GoogleSignInAccount> tcs;

            public SignInSuccessListener(TaskCompletionSource<GoogleSignInAccount> tcs)
            {
                this.tcs = tcs;
            }

            public void OnSuccess(Java.Lang.Object result)
            {
                //GoogleSignInAccount account = result.JavaCast<GoogleSignInAccount>();
                //tcs.SetResult(account);
            }
        }

        private class SignInFailureListener : Java.Lang.Object, IOnFailureListener
        {
            private TaskCompletionSource<GoogleSignInAccount> tcs;

            public SignInFailureListener(TaskCompletionSource<GoogleSignInAccount> tcs)
            {
                this.tcs = tcs;
            }

            public void OnFailure(Java.Lang.Exception e)
            {
                tcs.SetException(new Exception(e.Message));
            }
        }


        private void HandleSignInResultViejo(Task<GoogleSignInAccount> completedTask)
        {
            try
            {
                GoogleSignInAccount account = completedTask.Result;

                // Inicio de sesión exitoso, puedes proceder con la configuración de Google Fit API aquí
                Log.Info("SignInActivity", "signInResult:success idToken=" + account.IdToken);

                // Puedes iniciar una nueva actividad o realizar otras acciones aquí
                // Por ejemplo, iniciar una nueva actividad donde se configure Google Fit API
            }
            catch (Exception e)
            {
                // Error en el inicio de sesión
                Log.Wtf("SignInActivity", "signInResult:failed code=" + e.Message);
            }
        }

        public void DumpDataPoints(IEnumerable<DataPoint> dataPoints)
        {
            foreach (var point in dataPoints)
            {
                Console.WriteLine($"Data point: {point}");
                Console.WriteLine($"Type: {point.DataType.Name}");

            }
        }

        private const string ApplicationName = "Your App Name";

        private async System.Threading.Tasks.Task HandleSignInResultAsync(GoogleSignInAccount account)
        {
            if (account != null)
            {
                mGoogleSignInAccount = account;
                Log.Info("SignInActivity", "signInResult:success idToken=" + account.IdToken);


                var credential = GoogleCredential.FromAccessToken(account.IdToken);

                fitnessService = new FitnessService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });


                _stepReader = new GetFitnessInformation(account.IdToken);
                _stepReader.GetTotalStepsToday();

                GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                                .RequestEmail()
                                .RequestIdToken(Globales.clientIdWeb)
                                .RequestScopes(new Scope((string)FitnessClass.ScopeActivityRead), new Scope((string)FitnessClass.ScopeActivityReadWrite))
                                .RequestServerAuthCode(Globales.clientIdWeb, true)
                                .Build();




                if (account != null)
                {
                    mGoogleSignInAccount = account;
                    Log.Info("SignInActivity", "signInResult:success idToken=" + account.IdToken);

                    // Intercambia el ServerAuthCode por un Access Token
                    var authCode = account.ServerAuthCode;

                    // Usa el authCode para obtener el access token desde el servidor de Google
                    var tokenResponse = await ExchangeAuthCodeForAccessToken(authCode);

                    if (tokenResponse != null)
                    {
                        var accessToken = tokenResponse.AccessToken;

                        _stepReader = new GetFitnessInformation(accessToken);
                        _stepReader.GetTotalStepsToday();

                        // Aquí puedes continuar con la configuración de Google Fit API u otras acciones
                        // Por ejemplo, iniciar una nueva actividad donde se configure Google Fit API
                    }
                }
                else
                {
                    Log.Error("SignInActivity", "signInResult:failed");
                }

                // Aquí puedes continuar con la configuración de Google Fit API u otras acciones
                // Por ejemplo, iniciar una nueva actividad donde se configure Google Fit API
            }
            else
            {
                Log.Error("SignInActivity", "signInResult:failed");
            }
        }


        private async Task<TokenResponse> ExchangeAuthCodeForAccessToken(string authCode)
        {
            using (var client = new HttpClient())
            {
                var requestParams = new Dictionary<string, string>
        {
            { "code", authCode },
            { "client_id", Globales.clientIdWeb }, // Reemplaza YOUR_SERVER_CLIENT_ID con tu Client ID del servidor.
            { "client_secret", "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" }, // Reemplaza YOUR_SERVER_CLIENT_SECRET con tu Client Secret del servidor.
            { "redirect_uri", "" }, // Deja en blanco si no se utiliza.
            { "grant_type", "authorization_code" }
        };

                var requestContent = new FormUrlEncodedContent(requestParams);

                var response = await client.PostAsync("https://oauth2.googleapis.com/token", requestContent);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);
                }
            }

            return null;
        }
        public async System.Threading.Tasks.Task ReadDailyStepCount()
        {

            var startTimeMillis = new DateTimeOffset(DateTime.Today.AddDays(-1)).ToUnixTimeMilliseconds();
            var endTimeMillis = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            //// Calcular la fecha actual
            //DateTime currentDate = DateTime.Now;

            //// Calcular la fecha hace 10 días
            //DateTime pastDate = currentDate.AddDays(-10);

            // Convertir las fechas a milisegundos desde la época (1 de enero de 1970, 00:00:00 GMT)
            //long startTimeMillis = ConvertToUnixTimestamp(pastDate);
            //long endTimeMillis = ConvertToUnixTimestamp(currentDate);

            // Configurar la solicitud para leer datos de pasos del usuario
            DataReadRequest request = new DataReadRequest.Builder()
                .Aggregate(Android.Gms.Fitness.Data.DataType.TypeStepCountDelta, Android.Gms.Fitness.Data.DataType.AggregateStepCountDelta)
                .BucketByTime(1, Java.Util.Concurrent.TimeUnit.Days)
                .SetTimeRange(startTimeMillis, endTimeMillis, Java.Util.Concurrent.TimeUnit.Milliseconds)
                .Build();

            //// Obtener el cliente de historial de Google Fit
            //var fitnessHistoryClient = Fitness.GetHistoryClient(Application.Context, mGoogleSignInAccount);

            //// Ejecutar la solicitud para leer datos de pasos
            //DataReadResponse response = await fitnessHistoryClient.ReadDataAsync(request);

            //// Procesar la respuesta exitosa
            //if (response.Status.IsSuccess)
            //{
            //    foreach (var bucket in response.Buckets)
            //    {
            //        foreach (var dataSet in bucket.DataSets)
            //        {
            //            foreach (var dataPoint in dataSet.DataPoints)
            //            {
            //                foreach (var field in dataPoint.DataType.Fields)
            //                {
            //                    var value = dataPoint.GetValue(field);
            //                    Console.WriteLine($"Field: {field.Name} Value: {value}");
            //                }
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Error al leer datos: " + response.Status.StatusMessage);
            //}
        }


        #endregion

        private void GoogleHealthConnec()
        {
            //var ok = Com.Tactomotion.Utilidades.Funciones.Utilidades.ObternerTamanoPantalla(this);
            //var ok = Utilidades.ExtraerDatosGoogleHealth(this);

            // var ok = Utilidad.ExtraerDatosGConnect(this);

        }

        #region "Codigo Original Prueba Tecnica"
        private void ConnectGoogleFitOauth()
        {
            IFitnessManagerFactory fitnessManagerFactory = new FitnessManagerFactory();
            fitnessManager = fitnessManagerFactory.CreateFitnessManager();
            AuthorizeAndGetData();
        }


        private void AuthorizeAndGetData()
        {

            //CreatePendingIntentToOpenUrl(this,"https://www.google.com");


            try
            {

                //var pendingIntentFlags = (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                //         ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable
                //         : PendingIntentFlags.UpdateCurrent;
                //var pendingActivityIntent = PendingIntent.GetActivity(Application.Context, requestCode, activityIntent, pendingIntentFlags);
                //var pendingIntent = PendingIntent.GetBroadcast(Application.Context, requestCode, intent, pendingIntentFlags);




                //Activity currentActivity = Platform.CurrentActivity;
                //authenticator = fitnessManager.Initialize();
                //fitnessManager.AuthorizeOauhtGoogleFit(authenticator);

                //// En caso de que GetUI devuelva un Intent directamente
                //Android.Content.Intent authIntent = authenticator.GetUI(this);

                ///////////////////////////////////////////////////////////////////

                // Configurar el intent para abrir la actividad de autenticación

                Activity currentActivity = Platform.CurrentActivity;
                authenticator = fitnessManager.Initialize();
                fitnessManager.AuthorizeOauhtGoogleFit(authenticator);
                authenticator.GetUI(this);
                //  StartActivity(authenticator.GetUI(this));


                string url = $"{Globales.authorizeURL}?client_id={Uri.EscapeDataString(Globales.clientId)}&access_type=offline&consent=select_account&include_granted_scopes=true&response_type=code&scope={Globales.scope}&flowName=GeneralOAuthFlow&redirect_uri={Globales.redirectUri}";

                AbrirURL(url);

                //////////////////////////////////////////////////////////////////

                //Activity currentActivity = Platform.CurrentActivity;
                //authenticator = fitnessManager.Initialize();

                //// Si `AuthorizeOauhtGoogleFit` crea algún PendingIntent, asegúrate de que lo configure correctamente
                //fitnessManager.AuthorizeOauhtGoogleFit(authenticator);

                //// Obtén el intent de autenticación y asegúrate de que se use el flag IMMUTABLE
                //Android.Content.Intent authIntent = authenticator.GetUI(this);
                //PendingIntent pendingIntent = PendingIntent.GetActivity(
                //    currentActivity,
                //    0,
                //    authIntent,
                //    PendingIntentFlags.Immutable // Asegúrate de usar el flag IMMUTABLE
                //);


                //// Inicia la actividad usando el PendingIntent
                //pendingIntent.Send();


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        public async void AbrirURL(string url)
        {
            try
            {
                await Launcher.OpenAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir al abrir la URL
                Console.WriteLine($"Error al abrir la URL: {ex.Message}");
            }
        }


        private void CreatePendingIntentToOpenUrl(Context context, string url)
        {
            // Asegúrate de usar el Uri de Android.Net
            var uri = Android.Net.Uri.Parse(url);

            //var intent = new Intent(Intent.ActionView, uri);
            //var pendingIntent = PendingIntent.GetActivity(
            //    context,
            //    0,
            //    intent,
            //    PendingIntentFlags.Immutable // O PendingIntentFlags.Mutable si es necesario
            //);

            // Aquí puedes usar el pendingIntent como lo necesites
            // Por ejemplo, puedes crear una notificación que lo use
            // CreateNotificationWithPendingIntent(pendingIntent);
        }

        private void CreateNotificationWithPendingIntent(PendingIntent pendingIntent)
        {
            // Configurar el canal de notificación para Android Oreo y superior
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelId = "default";
                var channelName = "Default Channel";
                var importance = NotificationImportance.Default;
                var notificationChannel = new NotificationChannel(channelId, channelName, importance);

                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(notificationChannel);
            }

            // Crear la notificación
            var notificationBuilder = new NotificationCompat.Builder(this, "default")
                .SetContentTitle("Open Google")
                .SetContentText("Tap to open Google.com")
                .SetSmallIcon(Resource.Drawable.icon_feed)
                .SetContentIntent(pendingIntent)
                .SetAutoCancel(true);

            var notificationManagerCompat = NotificationManagerCompat.From(this);
            notificationManagerCompat.Notify(1, notificationBuilder.Build());
        }
        private FitnessService fitnessService;
        private static readonly string[] Scopes = { FitnessService.Scope.FitnessActivityRead };

        #endregion


    }


    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}



