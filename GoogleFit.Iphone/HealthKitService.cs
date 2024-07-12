using Foundation;
using HealthKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleFit.Iphone
{
    public class HealthKitService
    {
        HKHealthStore healthStore;

        public HealthKitService()
        {
            healthStore = new HKHealthStore();
        }

        private HKQuantityTypeIdentifier kind;


        [Obsolete]
        public void RequestAuthorization(Action<bool, NSError> completion)
        {
            if (!HKHealthStore.IsHealthDataAvailable)
            {
                // HealthKit no está disponible en este dispositivo
                return;
            }


            var readTypes = new NSSet(new[]
     {
            HKQuantityType.Create(HKQuantityTypeIdentifier.HeartRate),
            HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount),
            // Agrega aquí otros tipos de datos que deseas leer
        });

            var writeTypes = new NSSet(new[]
            {
            HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount),
            // Agrega aquí otros tipos de datos que deseas escribir
        });

            healthStore.RequestAuthorizationToShare(writeTypes, readTypes, (success, error) =>
            {
                if (success)
                {
                    // Permisos concedidos
                }
                else
                {
                    // Permisos denegados
                    Console.WriteLine(error.LocalizedDescription);
                }
            });

            //kind = new HKQuantityTypeIdentifier();
            //kind = HKQuantityTypeIdentifier.StepCount;
            //var readTypes = new NSSet(new[] {
            //    HKObjectType.GetQuantityType(kind.GetConstant())
            //});

            //var shareTypes = new NSSet(new[] {
            //    HKObjectType.GetQuantityType(kind.GetConstant())
            //});

            //healthStore.RequestAuthorizationToShare(shareTypes, readTypes, (success, error) =>
            //{
            //    completion(success, error);
            //});
        }

        public void ReadStepCount(Action<double, NSError> completion)
        {
            var sampleType = HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount);
            var startDate = NSDate.Now;
            var endDate = NSDate.Now;

            var predicate = HKQuery.GetPredicateForSamples(startDate, endDate, HKQueryOptions.StrictStartDate);
            var sortDescriptor = new NSSortDescriptor(HKSample.SortIdentifierStartDate, false);

            var query = new HKSampleQuery(sampleType, predicate, 0, new[] { sortDescriptor }, (query1, results, error) =>
            {
                if (results == null)
                {
                    completion(0, error);
                    return;
                }

                double totalSteps = 0;
                foreach (var result in results)
                {
                    if (result is HKQuantitySample sample)
                    {
                        totalSteps += sample.Quantity.GetDoubleValue(HKUnit.Count);
                    }
                }

                completion(totalSteps, null);
            });

            healthStore.ExecuteQuery(query);
        }

        public void WriteStepCount(double steps, Action<bool, NSError> completion)
        {
            var stepCountType = HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount);
            var stepCountQuantity = HKQuantity.FromQuantity(HKUnit.Count, steps);
            var now = NSDate.Now;

            var stepCountSample = HKQuantitySample.FromType(stepCountType, stepCountQuantity, now, now);

            healthStore.SaveObject(stepCountSample, (success, error) =>
            {
                completion(success, error);
            });
        }
    }
}
