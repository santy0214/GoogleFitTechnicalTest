using System;
using System.Collections.Generic;
using System.Linq;
using GoogleFit.Iphone;
using Foundation;
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

        [Export("window")]
        public UIWindow Window { get; set; }

        [Export("application:didFinishLaunchingWithOptions:")]
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {

            global::Xamarin.Forms.Forms.Init();


            var read = new NSSet(HKQuantityType.Create(HKQuantityTypeIdentifier.HeartRate));

            var healthstore = new HKHealthStore();
            healthstore.RequestAuthorizationToShare(new NSSet(), read, (f, error) => {
                if (error != null)
                {
                    Console.WriteLine(@"{0} error", error);
                } else
                {

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

                    healthKitService.RequestAuthorization((success, error1) =>
                    {
                        if (success)
                        {
                            Console.WriteLine("Authorization successful");
                        }
                        else
                        {
                            Console.WriteLine($"Authorization failed: {error1}");
                        }
                    });

                    healthKitService.ReadStepCount((steps, error1) =>
                    {
                        if (string.IsNullOrEmpty(error1))
                        {
                            Console.WriteLine($"Steps: {steps}");
                        }
                        else
                        {
                            Console.WriteLine($"Error reading steps: {error1}");
                        }
                    });

                    healthKitService.WriteStepCount(3500, (success, error1) =>
                    {
                        if (success)
                        {
                            Console.WriteLine("Successfully saved step count");
                        }
                        else
                        {
                            Console.WriteLine($"Error saving step count: {error1}");
                        }
                    });

                    LoadApplication(new App());

                    base.FinishedLaunching(app, options);
                }
      




            });

            return true;

        }

        [Export("application:configurationForConnectingSceneSession:options:")]
        public UISceneConfiguration GetConfiguration(UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
        {
            // Called when a new scene session is being created.
            // Use this method to select a configuration to create the new scene with.
            return UISceneConfiguration.Create("Default Configuration", connectingSceneSession.Role);
        }

        [Export("application:didDiscardSceneSessions:")]
        public void DidDiscardSceneSessions(UIApplication application, NSSet<UISceneSession> sceneSessions)
        {
            // Called when the user discards a scene session.
            // If any sessions were discarded while the application was not running, this will be called shortly after `FinishedLaunching`.
            // Use this method to release any resources that were specific to the discarded scenes, as they will not return.
        }
    }
}
