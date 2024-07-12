using Android.App;
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Request;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GoogleFit.Interfaces;
using GoogleFit.Models;
using Java.Util.Concurrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace LibraryAndroid
{
    public class GoogleFitAuth
    {
        private const int RC_SIGN_IN = 9001;

        private static GoogleSignInClient mGoogleSignInClient;
        public static GoogleApiClient googleApiClient;
        public static Context _context;
        public static Activity _activity;
        private string mail;
        public static Android.Gms.Tasks.Task task;

        public static void Initialize(Context context, Activity activity)
        {
            _context = context;
            _activity = activity;

            // Configurar las opciones de inicio de sesión de Google
            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                  .RequestIdToken(Globales.clientIdWeb)
                .RequestEmail()
                .Build();



            // Crear un cliente de inicio de sesión de Google
            mGoogleSignInClient = GoogleSignIn.GetClient(context, gso);

            // Iniciar el proceso de inicio de sesión
            Android.Content.Intent signInIntent = mGoogleSignInClient.SignInIntent;
            activity.StartActivityForResult(signInIntent, RC_SIGN_IN);


            //// Configurar las opciones de inicio de sesión de Google
            //GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
            //     .RequestEmail()
            //     .RequestIdToken(Globales.clientIdWeb) // Reemplaza con tu ID de cliente
            //     .RequestProfile()
            //     .RequestId()
            //     .RequestServerAuthCode(Globales.clientIdWeb, false)
            //     .Build();

            //// Construir GoogleApiClient con acceso a GoogleSignIn.API y las opciones especificadas por gso.
            //googleApiClient = new GoogleApiClient.Builder(_activity)
            //    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
            //    .Build();

            //// Crear un cliente de inicio de sesión de Google
            //mGoogleSignInClient = GoogleSignIn.GetClient(context, gso);

            //// Iniciar el proceso de inicio de sesión
            //Android.Content.Intent signInIntent = mGoogleSignInClient.SignInIntent;
            //activity.StartActivityForResult(signInIntent, RC_SIGN_IN);
        }

        [Obsolete]
        public static void BuildGoogleApiClient()
        {
            var connectionCallbacks = new MyConnectionCallbacks();
            var onConnectionFailedListener = new MyOnConnectionFailedListener();

            googleApiClient = new GoogleApiClient.Builder(_activity)
                .AddApi(FitnessClass.HISTORY_API)
                .AddScope(new Scope((string)FitnessClass.ScopeActivityRead))
                .AddScope(new Scope((string)FitnessClass.ScopeActivityReadWrite))
                .AddScope(new Scope((string)FitnessClass.ScopeLocationRead))
                .AddScope(new Scope((string)FitnessClass.ScopeLocationReadWrite))
                .AddScope(new Scope((string)FitnessClass.ScopeNutritionRead))
                .AddScope(new Scope((string)FitnessClass.ScopeNutritionReadWrite))
                .AddScope(new Scope((string)FitnessClass.ScopeBodyRead))
                .AddScope(new Scope((string)FitnessClass.ScopeBodyReadWrite))
                .AddScope(new Scope("https://www.googleapis.com/auth/fitness.heart_rate.read"))
                .AddScope(new Scope("https://www.googleapis.com/auth/fitness.body_temperature.read"))
                .AddApiIfAvailable(FitnessClass.HISTORY_API)
                .AddConnectionCallbacks(connectionCallbacks)
                .AddOnConnectionFailedListener(onConnectionFailedListener)
                .Build();

            googleApiClient.Connect();
            //while(true)
            //{
            //    if (googleApiClient.IsConnected)
            //    {
            //        break;
            //    }
            //    Thread.Sleep(2000);
            //}
        }

        public static async Task SyncDataAsync()
        {
            if (!googleApiClient.IsConnected)
            {
                googleApiClient.Connect();
            }

            var request = new DataDeleteRequest.Builder()
                .SetTimeInterval(2, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), Java.Util.Concurrent.TimeUnit.Milliseconds)
                .DeleteAllSessions()
                .Build();

            var result = await FitnessClass.HistoryApi.DeleteDataAsync(googleApiClient, request);

            if (result.Status.IsSuccess)
            {
                // Los datos se han sincronizado con éxito
            }
            else
            {
                // Manejo de errores
            }
        }

        public async static Task<object> ReadDailyTotalAsync(Android.Gms.Fitness.Data.DataType dataType)
        {
            var result = await FitnessClass.HistoryApi.ReadDailyTotalAsync(googleApiClient, dataType);

            if (result.Status.IsSuccess)
            {
                Console.WriteLine("Data read successfully");
                return result.Total;
            }
            else
            {
                Console.WriteLine($"Error reading data: {result.Status.StatusMessage}");
                return null;
            }
        }


        public static async Task<object> ReadDailyTotalFromLocalDeviceAsync(Android.Gms.Fitness.Data.DataType dataType)
        {
            var result = await FitnessClass.HistoryApi.ReadDailyTotalFromLocalDeviceAsync(googleApiClient, dataType);

            if (result.Status.IsSuccess)
            {
                Console.WriteLine("Data read successfully from local device");
                return result.Total;
            }
            else
            {
                Console.WriteLine($"Error reading data from local device: {result.Status.StatusMessage}");
                return null;
            }
        }

        public static void SingOut()
        {
            // Crear una instancia del cliente de Google Sign-In
            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestIdToken(Globales.clientIdWeb)
                .RequestEmail()
            .Build();

            GoogleSignInClient mGoogleSignInClient = GoogleSignIn.GetClient(_context, gso);

            mGoogleSignInClient.SignOutAsync();
            // Cierre de sesión exitoso, ahora puedes proceder a cambiar de cuenta
            Toast.MakeText(_context, "Sing Out", ToastLength.Short).Show();
        }

        [Obsolete]
        public static async Task<IList<DataSet>> ReadDataAsync(Android.Gms.Fitness.Data.DataType dataType, Android.Gms.Fitness.Data.DataType dataType2, DateTime startTime, DateTime endTime)
        {
            long startTimeMillis = ((DateTimeOffset)startTime).ToUnixTimeMilliseconds();
            long endTimeMillis = ((DateTimeOffset)endTime).ToUnixTimeMilliseconds();

            var readRequest = new DataReadRequest.Builder()
                .Aggregate(dataType, dataType2 )
                .BucketByTime(1, Java.Util.Concurrent.TimeUnit.Days)
                .SetTimeRange(startTimeMillis, endTimeMillis, Java.Util.Concurrent.TimeUnit.Milliseconds)
                .Build();

            var result = await FitnessClass.HistoryApi.ReadDataAsync(googleApiClient, readRequest);

            if (result.Status.IsSuccess)
            {
                Console.WriteLine("Data read successfully");
                List<DataSet> dataSets = new List<DataSet>();

                foreach (Bucket bucket in result.Buckets)
                {
                    foreach (DataSet dataSet in bucket.DataSets)
                    {
                        dataSets.Add(dataSet);
                    }
                }

                return dataSets;
            }
            else
            {
                Console.WriteLine($"Error reading data: {result.Status.StatusMessage}");
                return null;
            }
        }


        public static async Task<IList<DataSet>> ReadActivityMinutesAsync(DateTime startTime, DateTime endTime)
        {
            long startTimeMillis = ((DateTimeOffset)startTime).ToUnixTimeMilliseconds();
            long endTimeMillis = ((DateTimeOffset)endTime).ToUnixTimeMilliseconds();

            var readRequest = new DataReadRequest.Builder()
                .Aggregate(DataType.TypeActivitySegment, DataType.AggregateActivitySummary)
                .BucketByTime(1, Java.Util.Concurrent.TimeUnit.Days)
                .SetTimeRange(startTimeMillis, endTimeMillis, Java.Util.Concurrent.TimeUnit.Milliseconds)
                .Build();

            var result = await FitnessClass.HistoryApi.ReadDataAsync(googleApiClient, readRequest);

            if (result.Status.IsSuccess)
            {
                Console.WriteLine("Data read successfully");
                List<DataSet> dataSets = new List<DataSet>();

                foreach (Bucket bucket in result.Buckets)
                {
                    foreach (DataSet dataSet in bucket.DataSets)
                    {
                        dataSets.Add(dataSet);
                    }
                }

                return dataSets;
            }
            else
            {
                Console.WriteLine($"Error reading data: {result.Status.StatusMessage}");
                return null;
            }
        }

        [Obsolete]
        public static async System.Threading.Tasks.Task HandleSignInResultAsync(Android.Gms.Tasks.Task completedTask)
        {
            try
            {
                Globales.pasos = "";
                FitInfo fitInfo = new FitInfo();
                List<string> dataFit = new List<string>();
                GoogleSignInAccount account = (GoogleSignInAccount)completedTask.Result;

                dataFit.Add($"Account - {account.Email}\n\n");
                fitInfo.account = account.Email;
                var startTimeMillis = new DateTimeOffset(DateTime.Today.AddDays(-7)).ToUnixTimeMilliseconds();
                var endTimeMillis = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

                DataReadRequest request = new DataReadRequest.Builder()
                    .Aggregate(DataType.AggregateStepCountDelta)
                    .BucketByTime(1, TimeUnit.Days)
                    .SetTimeRange(startTimeMillis, endTimeMillis, TimeUnit.Milliseconds)
                    .Build();

                // Obtener el cliente de historial de Google Fit
                var fitnessHistoryClient = FitnessClass.HistoryApi;

                try
                {
                    await SyncDataAsync();

                    // Ejecutar la solicitud para leer datos de pasos

                    DataSource dataSource = new DataSource.Builder()

                                  .SetAppPackageName("com.google.android.gms")
                                  .SetDataType(DataType.TypeStepCountDelta)
                                  //.SetDataType(DataType.AggregateStepCountDelta)
                                  .SetType(DataSource.TypeDerived)
                                  .SetStreamName("estimated_steps")
                                  .Build();

                    fitInfo.steps = await GetDataGoogleFit(DataType.TypeStepCountDelta, DataType.TypeStepCountDelta, 86400000, TimeUnit.Milliseconds, startTimeMillis, endTimeMillis, dataSource);
                    fitInfo.calories = await GetDataGoogleFit(DataType.TypeCaloriesExpended, DataType.AggregateCaloriesExpended, 86400000, TimeUnit.Milliseconds, startTimeMillis, endTimeMillis);
                    fitInfo.minutes = await GetDataGoogleFit(DataType.TypeMoveMinutes, DataType.AggregateMoveMinutes, 86400000, TimeUnit.Milliseconds, startTimeMillis, endTimeMillis);
                    fitInfo.distance = await GetDataGoogleFit(DataType.TypeDistanceDelta, DataType.AggregateDistanceDelta, 86400000, TimeUnit.Milliseconds, startTimeMillis, endTimeMillis);
                    fitInfo.weight = await GetDataGoogleFit(DataType.TypeWeight, DataType.AggregateWeightSummary, 86400000, TimeUnit.Milliseconds, startTimeMillis, endTimeMillis);
                    fitInfo.height = await GetDataGoogleFit(DataType.TypeHeight, DataType.AggregateHeightSummary, 86400000, TimeUnit.Milliseconds, startTimeMillis, endTimeMillis);
                    fitInfo.heartRateBpm = await GetDataGoogleFit(DataType.TypeHeartRateBpm, DataType.AggregateHeartRateSummary, 86400000, TimeUnit.Milliseconds, startTimeMillis, endTimeMillis);
                    fitInfo.heartPoints = await GetDataGoogleFit(DataType.TypeHeartPoints, DataType.AggregateHeartPoints, 86400000, TimeUnit.Milliseconds, startTimeMillis, endTimeMillis);
                    //fitInfo.heartRateSummary = await GetDataGoogleFit(DataType.TypeHeartPoints, DataType.AggregateHeartRateSummary, 86400000, TimeUnit.Milliseconds, startTimeMillis, endTimeMillis);
                    fitInfo.speed = await GetDataGoogleFit(DataType.TypeSpeed, DataType.AggregateSpeedSummary, 86400000, TimeUnit.Milliseconds, startTimeMillis, endTimeMillis);
                    fitInfo.basalMetabolicRate = await GetDataGoogleFit(DataType.TypeBasalMetabolicRate, DataType.AggregateBasalMetabolicRateSummary, 86400000, TimeUnit.Milliseconds, startTimeMillis, endTimeMillis);
                    //fitInfo.locationTrack = await GetDataGoogleFit(DataType.TypeLocationTrack, DataType.AggregateLocationBoundingBox, 86400000, TimeUnit.Milliseconds, startTimeMillis, endTimeMillis);
                    fitInfo.activitySegment = await GetDataGoogleFit(DataType.TypeActivitySegment, DataType.AggregateActivitySummary, 86400000, TimeUnit.Milliseconds, startTimeMillis, endTimeMillis);
                    fitInfo.powerSample = await GetDataGoogleFit(DataType.TypePowerSample, DataType.AggregatePowerSummary, 86400000, TimeUnit.Milliseconds, startTimeMillis, endTimeMillis);
                    fitInfo.hydration = await GetDataGoogleFit(DataType.TypeHydration, DataType.AggregateHydration, 86400000, TimeUnit.Milliseconds, startTimeMillis, endTimeMillis);
                    fitInfo.nutrition = await GetDataGoogleFit(DataType.TypeNutrition, DataType.AggregateNutritionSummary, 86400000, TimeUnit.Milliseconds, startTimeMillis, endTimeMillis);




                }
                catch (Exception e)
                {

                }
                Globales.pasos = "";
                Globales.pasos += $"Steps: {fitInfo.steps} \n";
                Globales.pasos += $"Calories: {fitInfo.calories} \n";
                Globales.pasos += $"Minutes: {fitInfo.minutes} \n";
                Globales.pasos += $"Distance: {fitInfo.distance}  \n";
                Globales.pasos += $"Weight: {fitInfo.weight} \n";
                Globales.pasos += $"Height: {fitInfo.height} \n";
                Globales.pasos += $"Heart Rate Bpm: {fitInfo.heartRateBpm} \n";
                Globales.pasos += $"Heart Points: {fitInfo.heartPoints} \n";
                Globales.pasos += $"Heart Rate Summary: {fitInfo.heartRateSummary} \n";
                Globales.pasos += $"Speed: {fitInfo.speed} \n";
                Globales.pasos += $"Basal Metabolic Rate: {fitInfo.basalMetabolicRate} \n";
                Globales.pasos += $"Location Track: {fitInfo.locationTrack} \n";
                Globales.pasos += $"Activity Segment: {fitInfo.activitySegment} \n";
                Globales.pasos += $"Power Sample: {fitInfo.powerSample} \n";
                Globales.pasos += $"Hydration: {fitInfo.hydration} \n";
                Globales.pasos += $"Nutrition: {fitInfo.nutrition} \n";
                Globales.pasos += $"---------------------------------------------- \n";


                //SingOut();



                // Puedes iniciar una nueva actividad o realizar otras acciones aquí
                // Por ejemplo, iniciar una nueva actividad donde se configure Google Fit API
            }
            catch (Android.Gms.Common.Apis.ApiException apiEx)
            {
                // Maneja el error específico de la API
                Console.WriteLine($"Google Sign-In failed with code: {apiEx.StatusCode}");
            }
            catch (Exception e)
            {
                // Error en el inicio de sesión
                Console.WriteLine("SignInActivity", "signInResult:failed code=" + e.Message);
            }
        }

        [Obsolete]
        public static async System.Threading.Tasks.Task HandleSignInResultAsync1(Android.Gms.Tasks.Task completedTask)
        {
            try
            {
                GoogleSignInAccount account = (GoogleSignInAccount)completedTask.Result;

                //Log.Info("SignInActivity", "signInResult:success idToken=" + account.IdToken);



                DateTime startTime = new DateTime(2024, 06, 19);
                DateTime endTime = DateTime.Now;


                ReadActivityMinutesAsync(startTime, endTime);

                long startTimeMillis = ((DateTimeOffset)startTime).ToUnixTimeMilliseconds();
                long endTimeMillis = ((DateTimeOffset)endTime).ToUnixTimeMilliseconds();


                //var dataSets = await ReadDataAsync(Android.Gms.Fitness.Data.DataType.TypeStepCountDelta, Android.Gms.Fitness.Data.DataType.AggregateStepCountDelta, startTime, endTime);

                //if (dataSets != null)
                //{
                //    foreach (var dataSet in dataSets)
                //    {
                //        foreach (var dataPoint in dataSet.DataPoints)
                //        {
                //            foreach (var field in dataPoint.DataType.Fields)
                //            {
                //                var value = dataPoint.GetValue(field);
                //                Console.WriteLine($"Field: {field.Name}, Value: {value}");
                //            }
                //        }
                //    }
                //}
                //await ReadDailyTotalAsync(Android.Gms.Fitness.Data.DataType.AggregateStepCountDelta);

                //await ReadDailyTotalFromLocalDeviceAsync(Android.Gms.Fitness.Data.DataType.AggregateCaloriesExpended);
                //await ReadDailyTotalAsync(Android.Gms.Fitness.Data.DataType.AggregateCaloriesExpended);
                await SyncDataAsync();

                //var startTimeMillis = new DateTimeOffset(DateTime.Today.AddDays(-1)).ToUnixTimeMilliseconds();
                //var endTimeMillis = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

                //DateTime currentDate = DateTime.UtcNow;  // Obtiene la fecha y hora actuales en UTC
                //DateTime pastDate = DateTime.Today;   // Establece la fecha de ayer a las 12 AM en UTC

                //// Calcular la fecha actual
                //DateTime currentDate = DateTime.Now;

                //DateTime pastDate = currentDate.AddDays(-1);
                // Calcular la fecha hace 10 días

                //long startTimeMillis = ConvertToUnixTimestamp(pastDate);
                //long endTimeMillis = ConvertToUnixTimestamp(currentDate);

                // Crear una solicitud para leer datos de pasos estimados del usuario
                //DataReadRequest request = new DataReadRequest.Builder()
                //    .Aggregate(Android.Gms.Fitness.Data.DataType.AggregateStepCountDelta)
                //    .BucketByTime(1, Java.Util.Concurrent.TimeUnit.Days)
                //    .SetTimeRange(startTimeMillis, endTimeMillis, Java.Util.Concurrent.TimeUnit.Milliseconds)
                //    .Build();

                // Obtener el cliente de historial de Google Fit
                var fitnessHistoryClient = FitnessClass.HistoryApi;

                Globales.pasos = "";
                Globales.calories = "";
                try
                {
                    #region "Otro"




                    //    // Ejecutar la solicitud para leer datos de pasos
                    //    var response = await fitnessHistoryClient.ReadDataAsync(googleApiClient, request);

                    //    // Procesar la respuesta exitosa
                    //    if (response.Status.IsSuccess)
                    //    {
                    //        foreach (var dataPoint in response.Buckets)
                    //        {
                    //            foreach (var dp in dataPoint.DataSets)
                    //            {
                    //                foreach (var dpnt in dp.DataPoints)
                    //                {
                    //                    long value = dpnt.GetValue(Field.FieldSteps).AsInt();
                    //                    Console.WriteLine("Steps: " + value);
                    //                    Globales.pasos += ("Steps: " + value + "\n");
                    //                }
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Console.WriteLine("Error al leer datos: " + response.Status.StatusMessage);
                    //    }






                    //    startTimeMillis = new DateTimeOffset(DateTime.Today.AddDays(-1)).ToUnixTimeMilliseconds();
                    //    endTimeMillis = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

                    DataSource source = new DataSource.Builder()
                                        .SetAppPackageName("com.google.android.gms")
                                        .SetDataType(Android.Gms.Fitness.Data.DataType.TypeStepCountDelta)
                                        .SetType(DataSource.TypeDerived)
                                        .SetStreamName("estimated_steps")
                                        .Build();

                    //    var readRequest = new DataReadRequest.Builder()
                    //                      .Aggregate(source, Android.Gms.Fitness.Data.DataType.AggregateStepCountDelta)
                    //                      .BucketByTime(1, Java.Util.Concurrent.TimeUnit.Days)
                    //                      .SetTimeRange(startTimeMillis, endTimeMillis, Java.Util.Concurrent.TimeUnit.Milliseconds)
                    //                      .Build();

                    //    // Ejecutar la solicitud para leer datos de pasos
                    //    response = await fitnessHistoryClient.ReadDataAsync(googleApiClient, request);

                    //    // Procesar la respuesta exitosa
                    //    if (response.Status.IsSuccess)
                    //    {
                    //        foreach (var dataPoint in response.Buckets)
                    //        {
                    //            foreach (var dp in dataPoint.DataSets)
                    //            {
                    //                foreach (var dpnt in dp.DataPoints)
                    //                {
                    //                    long value = dpnt.GetValue(Field.FieldSteps).AsInt();
                    //                    Console.WriteLine("Steps: " + value);
                    //                    Globales.pasos += ("Steps: " + value + "\n");
                    //                }
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Console.WriteLine("Error al leer datos: " + response.Status.StatusMessage);
                    //    }
                    #endregion

                    //DataSource dataSource = new DataSource.Builder()
                    //              .SetAppPackageName("com.google.android.gms")
                    //              .SetDataType(Android.Gms.Fitness.Data.DataType.TypeStepCountDelta)
                    //              //.SetDataType(DataType.AggregateStepCountDelta)
                    //              .SetType(DataSource.TypeDerived)
                    //              .SetStreamName("estimated_steps")
                    //              .Build();

                    //var cal = Calendar.Instance;
                    //var now = new Date();

                    //cal.Time = now;
                    //var endTime = cal.TimeInMillis;

                    //cal.Add(CalendarField.DayOfWeek, -1);
                    //var startTime = cal.TimeInMillis;

                    var readRequest = new DataReadRequest.Builder()
                                    //.Aggregate(DataType.TypeStepCountDelta, DataType.AggregateStepCountDelta)
                                    //.Aggregate(dataSource, DataType.AggregateStepCountDelta)
                                    .Aggregate(source, Android.Gms.Fitness.Data.DataType.AggregateStepCountDelta)
                                    .BucketByTime(86400000, Java.Util.Concurrent.TimeUnit.Milliseconds)
                                    .SetTimeRange(startTimeMillis, endTimeMillis, Java.Util.Concurrent.TimeUnit.Milliseconds)
                                    .Build();

                    var dataReadResult = await FitnessClass.HistoryApi.ReadDataAsync(googleApiClient, readRequest);

                    if (dataReadResult.Status.IsSuccess)
                    {
                        foreach (var item in dataReadResult.DataSets)
                        {
                            var a = item;
                        }

                        foreach (Bucket item in dataReadResult.Buckets)
                        {
                            var t = item.DataSets;
                            var a = item;

                            foreach (var ds in item.DataSets)
                            {
                                foreach (var dp in ds.DataPoints)
                                {
                                    string device = dp.DataSource.Device?.Model ?? "Unknown";
                        
                                    foreach (var field in dp.DataType.Fields)
                                    {
                                        var b = dp.GetValue(field);
                                        Globales.pasos += ("Steps: " + b + "\n");
                                    }
                                }
                            }
                        }
                    }


                    //dataSource = new DataSource.Builder()
                    //                .SetAppPackageName("com.google.android.gms")
                    //                .SetDataType(Android.Gms.Fitness.Data.DataType.TypeCaloriesExpended)
                    //                .SetType(DataSource.TypeDerived)
                    //                .SetStreamName("merge_calories_expended")
                    //                .Build();

                    readRequest = new DataReadRequest.Builder()
                                    .Aggregate(Android.Gms.Fitness.Data.DataType.TypeCaloriesExpended, Android.Gms.Fitness.Data.DataType.AggregateCaloriesExpended)
                                    .BucketByTime(86400000, Java.Util.Concurrent.TimeUnit.Milliseconds)
                                    .SetTimeRange(startTimeMillis, endTimeMillis, Java.Util.Concurrent.TimeUnit.Milliseconds)
                                    .Build();


                    dataReadResult = await FitnessClass.HistoryApi.ReadDataAsync(googleApiClient, readRequest);

                    Console.WriteLine("Resultado de lectura obtenido: " + dataReadResult.Status.StatusMessage);

                    // Verificar si la solicitud fue exitosa
                    if (dataReadResult.Status.IsSuccess)
                    {
                        Console.WriteLine("Lectura de datos exitosa");
                        foreach (var bucket in dataReadResult.Buckets)
                        {
                            Console.WriteLine("Procesando bucket: " + bucket);
                            foreach (var dataSet in bucket.DataSets)
                            {
                                Console.WriteLine("Procesando dataSet: " + dataSet);
                                foreach (var dataPoint in dataSet.DataPoints)
                                {
                                    Console.WriteLine("Procesando dataPoint: " + dataPoint);
                                    foreach (var field in dataPoint.DataType.Fields)
                                    {
                                        var value = dataPoint.GetValue(field);
                                        Globales.calories += ("Calories: " + value + "\n");
                                        Console.WriteLine("Calorías: " + value);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Manejo de error en caso de que la solicitud no sea exitosa
                        Console.WriteLine("Error al leer los datos de calorías expended. Estado: " + dataReadResult.Status.StatusMessage);
                    }


                    readRequest = new DataReadRequest.Builder()
                                    .Aggregate(Android.Gms.Fitness.Data.DataType.TypeMoveMinutes, Android.Gms.Fitness.Data.DataType.AggregateMoveMinutes)
                                    .BucketByTime(86400000, Java.Util.Concurrent.TimeUnit.Milliseconds)
                                    .SetTimeRange(startTimeMillis, endTimeMillis, Java.Util.Concurrent.TimeUnit.Milliseconds)
                                    .Build();


                    dataReadResult = await FitnessClass.HistoryApi.ReadDataAsync(googleApiClient, readRequest);

                    Console.WriteLine("Resultado de lectura obtenido: " + dataReadResult.Status.StatusMessage);

                    // Verificar si la solicitud fue exitosa
                    if (dataReadResult.Status.IsSuccess)
                    {
                        Console.WriteLine("Lectura de datos exitosa");
                        foreach (var bucket in dataReadResult.Buckets)
                        {
                            Console.WriteLine("Procesando bucket: " + bucket);
                            foreach (var dataSet in bucket.DataSets)
                            {
                                Console.WriteLine("Procesando dataSet: " + dataSet);
                                foreach (var dataPoint in dataSet.DataPoints)
                                {

                                    string device = dataPoint.DataSource.Device?.Model ?? "Unknown";

                                    Console.WriteLine("Procesando dataPoint: " + dataPoint);
                                    foreach (var field in dataPoint.DataType.Fields)
                                    {
                                        var value = dataPoint.GetValue(field);
                                        Globales.minutes += ("Minutes: " + value + "\n");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Manejo de error en caso de que la solicitud no sea exitosa
                        Console.WriteLine("Error al leer los datos de calorías expended. Estado: " + dataReadResult.Status.StatusMessage);
                    }


                    readRequest = new DataReadRequest.Builder()
                                    .Aggregate(DataType.TypeDistanceDelta, Android.Gms.Fitness.Data.DataType.AggregateDistanceDelta)
                                    .BucketByTime(86400000, Java.Util.Concurrent.TimeUnit.Milliseconds)
                                    .SetTimeRange(startTimeMillis, endTimeMillis, Java.Util.Concurrent.TimeUnit.Milliseconds)
                                    .Build();


                    dataReadResult = await FitnessClass.HistoryApi.ReadDataAsync(googleApiClient, readRequest);

                    Console.WriteLine("Resultado de lectura obtenido: " + dataReadResult.Status.StatusMessage);

                    // Verificar si la solicitud fue exitosa
                    if (dataReadResult.Status.IsSuccess)
                    {
                        Console.WriteLine("Lectura de datos exitosa");
                        foreach (var bucket in dataReadResult.Buckets)
                        {
                            Console.WriteLine("Procesando bucket: " + bucket);
                            foreach (var dataSet in bucket.DataSets)
                            {
                                Console.WriteLine("Procesando dataSet: " + dataSet);
                                foreach (var dataPoint in dataSet.DataPoints)
                                {
                                    Console.WriteLine("Procesando dataPoint: " + dataPoint);
                                    foreach (var field in dataPoint.DataType.Fields)
                                    {
                                        var value = dataPoint.GetValue(field);
                                        Globales.minutes += ("Distancia Metros: " + value + "\n");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Manejo de error en caso de que la solicitud no sea exitosa
                        Console.WriteLine("Error al leer los datos de calorías expended. Estado: " + dataReadResult.Status.StatusMessage);
                    }


                    readRequest = new DataReadRequest.Builder()
                                    .Aggregate(DataType.TypeWeight, DataType.AggregateWeightSummary)
                                    .BucketByTime(86400000, Java.Util.Concurrent.TimeUnit.Milliseconds)
                                    .SetTimeRange(startTimeMillis, endTimeMillis, Java.Util.Concurrent.TimeUnit.Milliseconds)
                                    .Build();


                    dataReadResult = await FitnessClass.HistoryApi.ReadDataAsync(googleApiClient, readRequest);

                    Console.WriteLine("Resultado de lectura obtenido: " + dataReadResult.Status.StatusMessage);

                    // Verificar si la solicitud fue exitosa
                    if (dataReadResult.Status.IsSuccess)
                    {
                        Console.WriteLine("Lectura de datos exitosa");
                        foreach (var bucket in dataReadResult.Buckets)
                        {
                            Console.WriteLine("Procesando bucket: " + bucket);
                            foreach (var dataSet in bucket.DataSets)
                            {
                                Console.WriteLine("Procesando dataSet: " + dataSet);
                                foreach (var dataPoint in dataSet.DataPoints)
                                {
                                    Console.WriteLine("Procesando dataPoint: " + dataPoint);
                                    foreach (var field in dataPoint.DataType.Fields)
                                    {
                                        var value = dataPoint.GetValue(field);
                                        Globales.minutes += ("Distancia Metros: " + value + "\n");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Manejo de error en caso de que la solicitud no sea exitosa
                        Console.WriteLine("Error al leer los datos de calorías expended. Estado: " + dataReadResult.Status.StatusMessage);
                    }


                    var dataTypes = new DataType[] {
                                DataType.TypeWeight,
                                DataType.TypeBodyFatPercentage
                            };

                                        var metrics = new Dictionary<string, double?> {
                                { "Weight", null },
                                { "BMI", null },
                                { "BodyFat", null }
                            };

                    foreach (var dataType in dataTypes)
                    {
     

                        readRequest = new DataReadRequest.Builder()
                            .Read(dataType)
                            .SetTimeRange(startTimeMillis, endTimeMillis, TimeUnit.Milliseconds)
                            .Build();

                        var result = await FitnessClass.HistoryApi.ReadDataAsync(googleApiClient, readRequest);

                        if (result.Status.IsSuccess)
                        {
                            foreach (var dataSet in result.DataSets)
                            {
                                foreach (var dataPoint in dataSet.DataPoints)
                                {
                                    foreach (var field in dataPoint.DataType.Fields)
                                    {
                                        var value = dataPoint.GetValue(field).AsFloat();
                                        metrics[field.Name] = (double)value;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Error reading data: {result.Status.StatusMessage}");
                        }
                    }
                }
                catch (Exception e)
                {

                }
                Globales.pasos += (Globales.calories + Globales.minutes);

                //SingOut();
            }
            catch (Exception e)
            {
                // Error en el inicio de sesión
                //Log.Wtf("SignInActivity", "signInResult:failed code=" + e.Message);
            }
        }



        [Obsolete]
        private static async Task<string> GetDataGoogleFit(DataType dataType, DataType dataTypeOpcion, int duration, TimeUnit timeUnit, long startTimeMillis, long endTimeMillis, DataSource source = null)
        {
            string valueResponse = null;

            object readRequest = null;


            if (source != null)
            {
                readRequest = new DataReadRequest.Builder()
                            .Aggregate(source, dataTypeOpcion)
                            .BucketByTime(duration, timeUnit)
                            .SetTimeRange(startTimeMillis, endTimeMillis, timeUnit).Build();
            }
            else
            {
                readRequest = new DataReadRequest.Builder()
                           .Aggregate(dataType, dataTypeOpcion)
                           .BucketByTime(duration, timeUnit)
                           .SetTimeRange(startTimeMillis, endTimeMillis, timeUnit).Build();
            }
            string origen = "Sin Origen";
            try
            {
                var dataReadResult = await FitnessClass.HistoryApi.ReadDataAsync(googleApiClient, (DataReadRequest)readRequest);


                if (dataReadResult.Status.IsSuccess)
                {
                    foreach (var bucket in dataReadResult.Buckets)
                    {
                        foreach (var dataSet in bucket.DataSets)
                        {
                            foreach (var dataPoint in dataSet.DataPoints)
                            {

                                long startTimeNanos = dataPoint.GetStartTime(TimeUnit.Milliseconds);
                                long endTimeNanos = dataPoint.GetEndTime(TimeUnit.Milliseconds);
                                DateTime fechaInicio = ConvertirMilisegundosAFecha(startTimeNanos);
                                DateTime fechafin = ConvertirMilisegundosAFecha(endTimeNanos);
                                bool dispositivoEncontrado = true;
                                if(dataPoint.OriginalDataSource != null)
                                {
                                    if(dataPoint.OriginalDataSource.Device != null)
                                    {
                                        origen = dataPoint.OriginalDataSource.Device.Manufacturer + " - " + dataPoint.OriginalDataSource.Device.Model;
                                        dispositivoEncontrado = true;
                                    }
                                }
                                if (dispositivoEncontrado)
                                {
                                    foreach (var field in dataPoint.DataType.Fields)
                                    {
                                        var value = dataPoint.GetValue(field);

                                        if(field.Name == "intensity" && dataType.Equals(DataType.TypeHeartPoints))
                                        {
                                            valueResponse = origen + " | " + value.ToString() + "\n";
                                        }

                                        if (field.Name != "intensity" && !dataType.Equals(DataType.TypeHeartPoints))
                                        {
                                            if(valueResponse == null)
                                            {
                                                valueResponse = origen + " | " + value.ToString() + "\n";
                                            } else
                                            {
                                                valueResponse += ("             * " + origen + " | " + value.ToString() + "\n");

                                            }
                                        }


                                        // Si el valor del campo es un timestamp en nanosegundos, conviértelo a DateTime
                                        if (field.Name == "startTimeNanos" || field.Name == "endTimeNanos")
                                        {
                                            long timestampNanos = long.Parse(valueResponse);
                                            DateTime timestamp = DateTimeOffset.FromUnixTimeMilliseconds(timestampNanos / 1000000).DateTime;
                                            Console.WriteLine($"{field.Name}: {timestamp}");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"{field.Name}: {valueResponse}");
                                        }
                                    }
                                }
                                
                            }
                        }
                    }
                }
                else
                {
                    // Manejo de error en caso de que la solicitud no sea exitosa
                    Console.WriteLine("Error al leer los datos de calorías expended. Estado: " + dataReadResult.Status.StatusMessage);
                }
                return valueResponse;
            } catch (Exception e) { 
            }
            return "";
        }



        static DateTime ConvertirMilisegundosAFecha(long milisegundos)
        {
            var date = (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(milisegundos.ToString()));

            return date;
        }

        [Obsolete]
        private static async Task<string> GetDataGoogleFit1(DataType dataType, DataType dataTypeOpcion, int duration, TimeUnit timeUnit, long startTimeMillis, long endTimeMillis, DataSource source = null)
        {
            string valueResponse = null;

            object readRequest = null;


            if (source != null)
            {
                readRequest = new DataReadRequest.Builder()
                            .Aggregate(source, dataTypeOpcion)
                            .BucketByTime(duration, timeUnit)
                            .SetTimeRange(startTimeMillis, endTimeMillis, timeUnit).Build();
            }
            else
            {
                readRequest = new DataReadRequest.Builder()
                           .Aggregate(dataType, dataTypeOpcion)
                           .BucketByTime(duration, timeUnit)
                           .SetTimeRange(startTimeMillis, endTimeMillis, timeUnit).Build();
            }

            var dataReadResult = await FitnessClass.HistoryApi.ReadDataAsync(googleApiClient, (DataReadRequest)readRequest);


            if (dataReadResult.Status.IsSuccess)
            {
                foreach (var bucket in dataReadResult.Buckets)
                {
                    foreach (var dataSet in bucket.DataSets)
                    {
                        foreach (var dataPoint in dataSet.DataPoints)
                        {
                            long startTimeNanos = dataPoint.GetStartTime(TimeUnit.Nanoseconds);
                            long endTimeNanos = dataPoint.GetEndTime(TimeUnit.Nanoseconds);

                            foreach (var field in dataPoint.DataType.Fields)
                            {

                                var value = dataPoint.GetValue(field);
                                valueResponse = value.ToString();

                                // Si el valor del campo es un timestamp en nanosegundos, conviértelo a DateTime
                                if (field.Name == "startTimeNanos" || field.Name == "endTimeNanos")
                                {
                                    long timestampNanos = long.Parse(valueResponse);
                                    DateTime timestamp = DateTimeOffset.FromUnixTimeMilliseconds(timestampNanos / 1000000).DateTime;
                                    Console.WriteLine($"{field.Name}: {timestamp}");
                                }
                                else
                                {
                                    Console.WriteLine($"{field.Name}: {valueResponse}");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Manejo de error en caso de que la solicitud no sea exitosa
                Console.WriteLine("Error al leer los datos de calorías expended. Estado: " + dataReadResult.Status.StatusMessage);
            }
            return valueResponse;
        }


        public async System.Threading.Tasks.Task ReadDailyStepCountAsync()
        {

            // Calcular la fecha actual
            DateTime currentDate = DateTime.Now;

            // Calcular la fecha hace 10 días
            DateTime pastDate = currentDate.AddDays(-10);

            // Convertir las fechas a milisegundos desde la época (1 de enero de 1970, 00:00:00 GMT)
            long startTimeMillis = ConvertToUnixTimestamp(pastDate);
            long endTimeMillis = ConvertToUnixTimestamp(currentDate);

            // Crear una solicitud para leer datos de pasos del usuario
            DataReadRequest request = new DataReadRequest.Builder()
                .Aggregate(Android.Gms.Fitness.Data.DataType.TypeStepCountDelta, Android.Gms.Fitness.Data.DataType.AggregateStepCountDelta)
                .BucketByTime(1, Java.Util.Concurrent.TimeUnit.Days)
                .SetTimeRange(startTimeMillis, endTimeMillis, Java.Util.Concurrent.TimeUnit.Milliseconds)
                .Build();

            // Obtener el cliente de historial de Google Fit
            var fitnessHistoryClient = FitnessClass.HistoryApi;

            //BuildGoogleApiClient();


            try
            {
                // Ejecutar la solicitud para leer datos de pasos
                var response = await fitnessHistoryClient.ReadDataAsync(googleApiClient, request);

                // Procesar la respuesta exitosa
                if (response.Status.IsSuccess)
                {
                    foreach (var dataPoint in response.Buckets)
                    {
                        foreach (var dp in dataPoint.DataSets)
                        {
                            foreach (var dpnt in dp.DataPoints)
                            {
                                long value = dpnt.GetValue(Field.FieldSteps).AsInt();
                                Console.WriteLine("Steps: " + value);
                                Globales.pasos += ("Steps: " + value + "\n");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error al leer datos: " + response.Status.StatusMessage);
                }
            }
            catch (Exception e)
            {

            }

        }



        private long ConvertToUnixTimestamp(DateTime date)
        {
            return ((DateTimeOffset)date).ToUnixTimeMilliseconds();
        }



    }


}