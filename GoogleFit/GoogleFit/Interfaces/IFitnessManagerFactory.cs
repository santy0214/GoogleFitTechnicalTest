using GoogleFit.Models;

namespace GoogleFit.Services
{
    public interface IFitnessManagerFactory
    {
        IFitnessManager CreateFitnessManager();
    }

    public class FitnessManagerFactory : IFitnessManagerFactory
    {
        public IFitnessManager CreateFitnessManager()
        {
            return new FitnesDataConect(); 
        }
    }

}
