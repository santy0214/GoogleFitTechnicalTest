using GoogleFit.Droid;
using GoogleFit.Models;
using RestSharp.Authenticators;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Auth;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Android.Provider.Settings;

namespace GoogleFit.ViewModels
{
    public class AboutViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private GetFitnessInformation _fitReader;
        public AboutViewModel()
        {
            Title = "History Fitness Last Day";
            TextLabel = "";
            OpenWebCommand = new Command(async () => await LoadGoogleFitAsync());
        }


        private string _textLabel;

        public string TextLabel
        {
            get { return _textLabel; }
            set
            {
                if (_textLabel != value)
                {
                    _textLabel = value;
                    OnPropertyChanged();
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task LoadGoogleFitAsync()
        {

            _fitReader = new GetFitnessInformation(Globales.Token);

            int totalSteps = await _fitReader.GetTotalStepsToday();
            int totalCardio = await _fitReader.GetTotalCardioPointsToday();
            int totalCalories = await _fitReader.GetTotalCaloriesToday();
            int totalMinutes = await _fitReader.GetTotalActiveMinutesToday();
            TextLabel = "Total Steps: " + totalSteps + "\n\n" +
                     "Total Calories: " + totalCalories + "\n\n" +
                     "Total Activity Minutes: " + totalMinutes + "\n\n" +
                     "Total Cardiovascular Points: " + totalCardio;


        }


        public ICommand OpenWebCommand { get; }
    }
}