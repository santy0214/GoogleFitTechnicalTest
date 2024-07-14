using System;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(GoogleFit.Iphone.HealthKitServiceImplementation))]
namespace GoogleFit.Iphone
{
    public class HealthKitServiceImplementation : IHealthKitService
    {
        HealthKitService healthKitService = new HealthKitService();

        public void RequestAuthorization(Action<bool, string> completion)
        {
            healthKitService.RequestAuthorization((success, error) =>
            {
                completion(success, error?.LocalizedDescription);
            });
        }

        public void ReadStepCount(Action<double, string> completion)
        {
            healthKitService.ReadStepCount((steps, error) =>
            {
                completion(steps, error?.LocalizedDescription);
            });
        }

        public void WriteStepCount(double steps, Action<bool, string> completion)
        {
            //healthKitService.WriteStepCount(steps, (success, error) =>
            //{
            //    completion(success, error?.LocalizedDescription);
            //});
        }
    }
}