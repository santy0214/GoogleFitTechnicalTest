using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using GoogleFit.Iphone;
using HealthKit;
using UIKit;
using Xamarin.Forms;

namespace GoogleFit.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //

        HKHealthStore healthKitStore;


        void ReactToHealthCarePermissions(bool success, NSError error)
        {
            if (success)
            {
                // Permission granted
                Console.WriteLine("Permission granted to write heart rate data.");
            }
            else
            {
                // Permission denied
                Console.WriteLine($"Error: {error.LocalizedDescription}");
            }
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();


           string paquete = NSBundle.MainBundle.BundleIdentifier;


            // Initialize the HealthKit store
            healthKitStore = new HKHealthStore();

            //Request / Validate that the app has permission to store heart-rate data
            var heartRateId = HKQuantityTypeIdentifierKey.HeartRate;
            var heartRateType = HKObjectType.GetQuantityType(heartRateId);
            var typesToWrite = new NSSet(new[] { heartRateType });
            //We aren't reading any data for this sample
            var typesToRead = new NSSet();
            healthKitStore.RequestAuthorizationToShare(
                typesToWrite,
                typesToRead,
                ReactToHealthCarePermissions);

            var healthKitService = DependencyService.Get<IHealthKitService>();

            healthKitService.RequestAuthorization((success, error) =>
            {
                if (success)
                {
                    Console.WriteLine("Authorization successful");
                }
                else
                {
                    Console.WriteLine($"Authorization failed: {error}");
                }
            });

            healthKitService.ReadStepCount((steps, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Steps: {steps}");
                }
                else
                {
                    Console.WriteLine($"Error reading steps: {error}");
                }
            });

            healthKitService.WriteStepCount(1000, (success, error) =>
            {
                if (success)
                {
                    Console.WriteLine("Successfully saved step count");
                }
                else
                {
                    Console.WriteLine($"Error saving step count: {error}");
                }
            });

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
