using GoogleFit.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace GoogleFit.Models
{
    public class ChartViewModel : INotifyPropertyChanged
    {
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

        public ChartViewModel()
        {
            Data = new List<DataItem>() {
                new DataItem() { Argument = new DateTime(2018, 1, 1), Value = -17.5 },
                new DataItem() { Argument = new DateTime(2018, 1, 10), Value = -1.4 },
                new DataItem() { Argument = new DateTime(2018, 1, 20), Value = -22 },
                new DataItem() { Argument = new DateTime(2018, 1, 30), Value = -26.2 },
                new DataItem() { Argument = new DateTime(2018, 2, 10), Value = -17.5 },
                new DataItem() { Argument = new DateTime(2018, 2, 20), Value = -15.7 },
                new DataItem() { Argument = new DateTime(2018, 2, 28), Value = -7.8 },
                new DataItem() { Argument = new DateTime(2018, 3, 10), Value = -8.8 },
                new DataItem() { Argument = new DateTime(2018, 3, 20), Value = 1.3 },
                new DataItem() { Argument = new DateTime(2018, 3, 30), Value = -7.5 },
                new DataItem() { Argument = new DateTime(2018, 4, 10), Value = 1.5 },
                new DataItem() { Argument = new DateTime(2018, 4, 20), Value = 8.5 },
                new DataItem() { Argument = new DateTime(2018, 4, 30), Value = 11 },
                new DataItem() { Argument = new DateTime(2018, 5, 10), Value = 12.2 },
                new DataItem() { Argument = new DateTime(2018, 5, 20), Value = 13.7 },
                new DataItem() { Argument = new DateTime(2018, 5, 30), Value = 8.3 },
                new DataItem() { Argument = new DateTime(2018, 6, 10), Value = 15.3 },
                new DataItem() { Argument = new DateTime(2018, 6, 20), Value = 19.1 },
                new DataItem() { Argument = new DateTime(2018, 6, 30), Value = 22.3 },
                new DataItem() { Argument = new DateTime(2018, 7, 10), Value = 22.2 },
                new DataItem() { Argument = new DateTime(2018, 7, 20), Value = 24.5 },
                new DataItem() { Argument = new DateTime(2018, 7, 30), Value = 21.4 },
                new DataItem() { Argument = new DateTime(2018, 8, 10), Value = 21.2 },
                new DataItem() { Argument = new DateTime(2018, 8, 20), Value = 15.6 },
                new DataItem() { Argument = new DateTime(2018, 8, 30), Value = 15 },
            };


      //      GdpValueForUSA = new CountryGdp(
      //    "USA",
      //    new GdpValue(new DateTime(2017, 1, 1), 19.391),
      //    new GdpValue(new DateTime(2016, 1, 1), 18.624),
      //    new GdpValue(new DateTime(2015, 1, 1), 18.121),
      //    new GdpValue(new DateTime(2014, 1, 1), 17.428),
      //    new GdpValue(new DateTime(2013, 1, 1), 16.692),
      //    new GdpValue(new DateTime(2012, 1, 1), 16.155),
      //    new GdpValue(new DateTime(2011, 1, 1), 15.518),
      //    new GdpValue(new DateTime(2010, 1, 1), 14.964),
      //    new GdpValue(new DateTime(2009, 1, 1), 14.419),
      //    new GdpValue(new DateTime(2008, 1, 1), 14.719),
      //    new GdpValue(new DateTime(2007, 1, 1), 14.478)
      //);
      //      GdpValueForChina = new CountryGdp(
      //          "China",
      //          new GdpValue(new DateTime(2017, 1, 1), 12.238),
      //          new GdpValue(new DateTime(2016, 1, 1), 11.191),
      //          new GdpValue(new DateTime(2015, 1, 1), 11.065),
      //          new GdpValue(new DateTime(2014, 1, 1), 10.482),
      //          new GdpValue(new DateTime(2013, 1, 1), 9.607),
      //          new GdpValue(new DateTime(2012, 1, 1), 8.561),
      //          new GdpValue(new DateTime(2011, 1, 1), 7.573),
      //          new GdpValue(new DateTime(2010, 1, 1), 6.101),
      //          new GdpValue(new DateTime(2009, 1, 1), 5.110),
      //          new GdpValue(new DateTime(2008, 1, 1), 4.598),
      //          new GdpValue(new DateTime(2007, 1, 1), 3.552)
      //      );
      //      GdpValueForJapan = new CountryGdp(
      //          "Japan",
      //          new GdpValue(new DateTime(2017, 1, 1), 4.872),
      //          new GdpValue(new DateTime(2016, 1, 1), 4.949),
      //          new GdpValue(new DateTime(2015, 1, 1), 4.395),
      //          new GdpValue(new DateTime(2014, 1, 1), 4.850),
      //          new GdpValue(new DateTime(2013, 1, 1), 5.156),
      //          new GdpValue(new DateTime(2012, 1, 1), 6.203),
      //          new GdpValue(new DateTime(2011, 1, 1), 6.156),
      //          new GdpValue(new DateTime(2010, 1, 1), 5.700),
      //          new GdpValue(new DateTime(2009, 1, 1), 5.231),
      //          new GdpValue(new DateTime(2008, 1, 1), 5.038),
      //          new GdpValue(new DateTime(2007, 1, 1), 4.515)
      //      );
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}