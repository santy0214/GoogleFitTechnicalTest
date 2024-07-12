using GoogleFit.Models;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoogleFit.Views
{
    public partial class HistoryFitnessLastDay : ContentPage
    {
        public HistoryFitnessLastDay()
        {

            DevExpress.XamarinForms.Charts.Initializer.Init();
            BindingContext = new ChartViewModel();

            InitializeComponent();
        }
    }
}