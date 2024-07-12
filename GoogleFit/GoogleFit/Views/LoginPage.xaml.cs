using DevExpress.XamarinForms.Charts;
using GoogleFit.Models;
using GoogleFit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoogleFit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            DevExpress.XamarinForms.Charts.Initializer.Init();
           // BindingContext = new ChartViewModel();
this.BindingContext = new LoginViewModel();
            InitializeComponent();

            

            
        }

        public List<DataItem> Data { get; }

    }

    public class DataItem
    {
        public DateTime Argument { get; set; }
        public double Value { get; set; }
    }

    public class CountryGdp
    {
        public string CountryName { get; }
        public IList<GdpValue> Values { get; }

        public CountryGdp(string country, params GdpValue[] values)
        {
            this.CountryName = country;
            this.Values = new List<GdpValue>(values);
        }
    }

    public class GdpValue
    {
        public string Year { get; }
        public double Value { get; }

        public GdpValue(string year, double value)
        {
            this.Year = year;
            this.Value = value;
        }
    }
}


/*
 * 
 * 
 * 
 * 
 * 
 *  <dxc:CrosshairHintBehavior GroupHeaderTextPattern="{}{A$YYYY}" MaxSeriesCount="3">
     
 </dxc:CrosshairHintBehavior>
    public class DataItem
    {
        public DateTime Argument { get; set; }
        public double Value { get; set; }
    }

    public class CountryGdp
    {
        public string CountryName { get; }
        public IList<GdpValue> Values { get; }

        public CountryGdp(string country, params GdpValue[] values)
        {
            this.CountryName = country;
            this.Values = new List<GdpValue>(values);
        }
    }

    public class GdpValue
    {
        public DateTime Year { get; }
        public double Value { get; }

        public GdpValue(DateTime year, double value)
        {
            this.Year = year;
            this.Value = value;
        }
    }
*/