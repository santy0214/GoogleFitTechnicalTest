

using System;
namespace GoogleFit.Iphone
{
    public interface IHealthKitService
    {
        void RequestAuthorization(Action<bool, string> completion);
        void ReadStepCount(Action<double, string> completion);
        void WriteStepCount(double steps, Action<bool, string> completion);
    }
}
