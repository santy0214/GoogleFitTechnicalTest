using GoogleFit.Droid;
using GoogleFit.Interfaces;
using GoogleFit.Models;
using GoogleFit.Services;
using GoogleFit.Views;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Auth;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoogleFit.ViewModels
{
    public class AboutViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private GetFitnessInformation _fitReader;
        private IFitnessManager fitnessManager;

        private List<DataItem> data;

        public CountryGdp GdpValueForUSA { get; }
        public CountryGdp GdpValueForChina { get; }
        public CountryGdp GdpValueForJapan { get; }


        public List<DataItem> Data
        {
            get => data;
            set
            {
                data = value;
                OnPropertyChanged();
            }
        }

    
        public AboutViewModel()
        {


            //Data = new List<DataItem>() {
            //    new DataItem() { Argument = new DateTime(2018, 1, 1), Value = -17.5 },
            //    new DataItem() { Argument = new DateTime(2018, 1, 10), Value = -1.4 },
            //    new DataItem() { Argument = new DateTime(2018, 1, 20), Value = -22 },
            //    new DataItem() { Argument = new DateTime(2018, 1, 30), Value = -26.2 },
            //    new DataItem() { Argument = new DateTime(2018, 2, 10), Value = -17.5 },
            //    new DataItem() { Argument = new DateTime(2018, 2, 20), Value = -15.7 },
            //    new DataItem() { Argument = new DateTime(2018, 2, 28), Value = -7.8 },
            //    new DataItem() { Argument = new DateTime(2018, 3, 10), Value = -8.8 },
            //    new DataItem() { Argument = new DateTime(2018, 3, 20), Value = 1.3 },
            //    new DataItem() { Argument = new DateTime(2018, 3, 30), Value = -7.5 },
            //    new DataItem() { Argument = new DateTime(2018, 4, 10), Value = 1.5 },
            //    new DataItem() { Argument = new DateTime(2018, 4, 20), Value = 8.5 },
            //    new DataItem() { Argument = new DateTime(2018, 4, 30), Value = 11 },
            //    new DataItem() { Argument = new DateTime(2018, 5, 10), Value = 12.2 },
            //    new DataItem() { Argument = new DateTime(2018, 5, 20), Value = 13.7 },
            //    new DataItem() { Argument = new DateTime(2018, 5, 30), Value = 8.3 },
            //    new DataItem() { Argument = new DateTime(2018, 6, 10), Value = 15.3 },
            //    new DataItem() { Argument = new DateTime(2018, 6, 20), Value = 19.1 },
            //    new DataItem() { Argument = new DateTime(2018, 6, 30), Value = 22.3 },
            //    new DataItem() { Argument = new DateTime(2018, 7, 10), Value = 22.2 },
            //    new DataItem() { Argument = new DateTime(2018, 7, 20), Value = 24.5 },
            //    new DataItem() { Argument = new DateTime(2018, 7, 30), Value = 21.4 },
            //    new DataItem() { Argument = new DateTime(2018, 8, 10), Value = 21.2 },
            //    new DataItem() { Argument = new DateTime(2018, 8, 20), Value = 15.6 },
            //    new DataItem() { Argument = new DateTime(2018, 8, 30), Value = 15 },
            //};


            GdpValueForUSA = new CountryGdp(
                      "Steps",
                      new GdpValue("Monday", 1219.391),
                      new GdpValue("Tuesday", 4018.624),
                      new GdpValue("Wednesday", 3518.121),
                      new GdpValue("Thursday", 1516.692),
                      new GdpValue("Friday", 2600.155),
                      new GdpValue("Saturday", 1200.155),
                      new GdpValue("Sunday", 1100.155)
                  );
            //GdpValueForUSA = new CountryGdp(
            //    "GdpValueForUSA",
            //    new GdpValue(new DateTime(2017, 1, 1), 12.238),
            //    new GdpValue(new DateTime(2016, 1, 1), 11.191),
            //    new GdpValue(new DateTime(2015, 1, 1), 11.065),
            //    new GdpValue(new DateTime(2014, 1, 1), 10.482),
            //    new GdpValue(new DateTime(2013, 1, 1), 9.607),
            //    new GdpValue(new DateTime(2012, 1, 1), 8.561),
            //    new GdpValue(new DateTime(2011, 1, 1), 7.573),
            //    new GdpValue(new DateTime(2010, 1, 1), 6.101),
            //    new GdpValue(new DateTime(2009, 1, 1), 5.110),
            //    new GdpValue(new DateTime(2008, 1, 1), 4.598),
            //    new GdpValue(new DateTime(2007, 1, 1), 3.552)
            //);
            //GdpValueForJapan = new CountryGdp(
            //    "Japan",
            //    new GdpValue(new DateTime(2017, 1, 1), 4.872),
            //    new GdpValue(new DateTime(2016, 1, 1), 4.949),
            //    new GdpValue(new DateTime(2015, 1, 1), 4.395),
            //    new GdpValue(new DateTime(2014, 1, 1), 4.850),
            //    new GdpValue(new DateTime(2013, 1, 1), 5.156),
            //    new GdpValue(new DateTime(2012, 1, 1), 6.203),
            //    new GdpValue(new DateTime(2011, 1, 1), 6.156),
            //    new GdpValue(new DateTime(2010, 1, 1), 5.700),
            //    new GdpValue(new DateTime(2009, 1, 1), 5.231),
            //    new GdpValue(new DateTime(2008, 1, 1), 5.038),
            //    new GdpValue(new DateTime(2007, 1, 1), 4.515)
            //);

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
            ConnectGoogleFitOauth();

            //_fitReader = new GetFitnessInformation(Globales.Token);

            //int totalSteps = await _fitReader.GetTotalStepsToday();
            //int totalCardio = await _fitReader.GetTotalCardioPointsToday();
            //int totalCalories = await _fitReader.GetTotalCaloriesToday();
            //int totalMinutes = await _fitReader.GetTotalActiveMinutesToday();
            //TextLabel = "Total Steps: " + totalSteps + "\n\n" +
            //         "Total Calories: " + totalCalories + "\n\n" +
            //         "Total Activity Minutes: " + totalMinutes + "\n\n" +
            //         "Total Cardiovascular Points: " + totalCardio + "\n\n";
            TextLabel += Globales.pasos;

        }


        private void ConnectGoogleFitOauth()
        {
            IFitnessManagerFactory fitnessManagerFactory = new FitnessManagerFactory();
            fitnessManager = fitnessManagerFactory.CreateFitnessManager();
            AuthorizeAndGetData();


        }

        private void AuthorizeAndGetData()
        {
            try
            {
                var currentActivity = DependencyServiceProvider.ServiceProvider.GetService<ICurrentActivity>().GetCurrentActivity();

                DependencyServiceProvider.ServiceProvider.GetService<ICurrentActivity>().NewConnectGoogleFit();

                //authenticator = fitnessManager.Initialize();
                fitnessManager.NewConnectGoogleFit();

                DependencyServiceProvider.ServiceProvider.GetService<ICurrentActivity>().GetAuthenticator();
                //Activity currentActivity = Platform.CurrentActivity;



            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public ICommand OpenWebCommand { get; }
    }
}