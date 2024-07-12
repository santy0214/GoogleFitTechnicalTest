
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using Xamarin.Essentials;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Fitness.v1;
using Google.Apis.Services;
using Newtonsoft.Json.Linq;
using GoogleFit.Models;
using Google.Apis.Auth.OAuth2.Responses;
using Newtonsoft.Json;
using Google.Apis.Fitness.v1.Data;

namespace GoogleFit.Droid
{
    public class GetFitnessInformation
    {

        private readonly string _accessToken;

        public GetFitnessInformation(string accessToken)
        {
            _accessToken = accessToken;
        }

        public async Task<int> GetTotalStepsToday()
        {
            try
            {
                var steps = await GetDataForDataType("com.google.step_count.delta", "estimated_steps", "intVal");
                return (int)steps;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> GetTotalCaloriesToday()
        {
            try
            {
                var calories = await GetDataForDataType("com.google.calories.expended", "merge_calories_expended", "fpVal");
                return (int)calories;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> GetTotalActiveMinutesToday()
        {
            try
            {
                var activeMinutes = await GetDataForDataType("com.google.active_minutes", "merge_active_minutes", "intVal");
                return (int)activeMinutes;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> GetTotalCardioPointsToday()
        {
            try
            {
                var cardioPoints = await GetDataForDataType("com.google.heart_minutes", "merge_step_deltas", "intVal");
                return (int)cardioPoints;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private async Task<double> GetDataForDataType(string dataTypeName, string type, string field)
        {

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
                var startTimeMillis = new DateTimeOffset(DateTime.Today.AddDays(-1)).ToUnixTimeMilliseconds();
                var endTimeMillis = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

                var requestData = new
                {
                    aggregateBy = new[]
                    {
                        new
                        {
                            dataTypeName = dataTypeName,
                            dataSourceId = "derived:" + dataTypeName + ":com.google.android.gms:" + type
                        }
                    },
                    startTimeMillis = startTimeMillis,
                    endTimeMillis = endTimeMillis
                };

                var jsonRequest = JObject.FromObject(requestData);

                var response = await client.PostAsync(Globales.GoogleFitApiUrl, new StringContent(jsonRequest.ToString()));

                var responseContent = await response.Content.ReadAsStringAsync();
                if (!responseContent.Contains("Unauthorized") && !responseContent.Contains("UNAUTHENTICATED"))
                {
                    Globales.notConnect = true;
                    var jsonResponse = JObject.Parse(responseContent);
                    var error = jsonResponse["error"]?["code"]?.Value<int>();

                    if (error == null)
                    {
                        var bucket = jsonResponse["bucket"]?[0];
                        var dataset = bucket?["dataset"]?[0];

                        if (dataset != null)
                        {
                            var value = dataset["point"][0]["value"][0][field];
                            return (double)value;
                        }
                    }
                }
                else
                {
                    //Preferences.Remove("GoogleFitAccessToken", string.Empty);
                    //Preferences.Remove("GoogleFitRefreshToken", string.Empty);
                    //Globales.notConnect = false;
                }

                return 0;
            }
        }
    }
}