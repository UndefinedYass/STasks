using STasks.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace STasks.ViewModel
{
    public class ClassViewModel : BaseDependencyProgViewModel ,ITabContent
    {
        public ClassViewModel(Class model): base(model)
        {
            SeriesVMS = new ObservableCollection<SeriesViewModel>(model.Series.Select((serie) => new SeriesViewModel(serie)));
            Name = model.Name;
            CurrentSelectedSeriesIndex = 0;
            ModelClass = model;
        }

        public ClassViewModel():base(Common.Mock.GetDummyClass())
        {
            ModelClass = Common.Mock.GetDummyClass("d-time-class");
            SeriesVMS = new ObservableCollection<SeriesViewModel>(ModelClass.Series.Select((s) => new SeriesViewModel(s)));
            Name = "ELECT";
        }

        private string _Name;
        public string Name
        {
            set { _Name = value; notif(nameof(Name)); }
            get { return _Name; }
        }
       


        private int _CurrentSelectedSeriesIndex;
        public int CurrentSelectedSeriesIndex
        {
            set { Trace.WriteLine($"setting tab inedx to {value} from class: {this.Name}"); _CurrentSelectedSeriesIndex = value; notif(nameof(CurrentSelectedSeriesIndex)); }
            get { return _CurrentSelectedSeriesIndex; }
        }

        /// <summary>
        /// the order in which the properties of a control are assigned in xaml is a big deal!
        /// i binded the selectedIndex before binding the items source and it cost me alot to figure it out
        /// glad that I eventualy did, and I knew it!! like that ahhhh momnet
        ///
        /// </summary>

        public ObservableCollection<SeriesViewModel> SeriesVMS { get; set; }

        public bool IsSpecialHomeTab
        {
            get
            {
                return false;
            }
        }
        public bool IsDirty
        {
            get
            {
                return false;
            }
        }

        public string Title
        {
            get
            {
                return Name;
            }
        }

        public Class ModelClass { get; internal set; }
    }
}
