using STasks.Model;
using STasks.Model.Bases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace STasks.ViewModel
{
    public class ClassViewModel : BaseProgressObjectViewModel ,ITabContent
    {
        public ClassViewModel(Class model): base(model)
        {
            SeriesVMS = new ObservableCollection<SeriesViewModel>(model.Series.Select((serie) => new SeriesViewModel(serie as Series)));
            notif(nameof(Name));
            CurrentSelectedSeriesIndex = 0;
            ModelClass = model;
            ModelClass.PropertyChanged += (s, e) =>
            {
                notif(nameof(Name));
                notif(nameof(Title));
            };
        }

        public ClassViewModel():base(( Common.Mock.GetDummyClass()))
        {
            //design-time
            ModelClass = Common.Mock.GetDummyClass("d-time-class");
            SeriesVMS = new ObservableCollection<SeriesViewModel>(ModelClass.Series.Select((s) => new SeriesViewModel(s as Series)));
            notif(nameof(Name));
        }


        internal override void handleContainerAdded(STContainer newContainer)
        {
            SeriesVMS.Add(new SeriesViewModel((Series)newContainer) { IsRenaming = true });
        }
        internal override void handleContainerRemoved(STContainer removedContainer)
        {
            Series rem = removedContainer as Series;
            SeriesViewModel removedOne = SeriesVMS.FirstOrDefault((cc) => cc.SeriesModel == rem);
            SeriesVMS.Remove(removedOne);
        }


        public string Name
        {
            get { return ModelClass?.Name; }
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
                return ModelClass.Name;
            }
        }




        public Class ModelClass { get; internal set; }




        public ICommand AddSeriesCommand
        {
            get { return new Common.MICommand(HandleAddSeriesCommand); }
        }
        public ICommand ClearSeriesCommand
        {
            get { return new Common.MICommand(HandleClearSeriesCommand); }
        }

        private void HandleClearSeriesCommand()
        {
            ModelClass.Containers.Clear();

        }

        private void HandleAddSeriesCommand()
        {
            int series_max_index = 0;
            if (ModelClass.Series.Any())
            {
                series_max_index = ModelClass.Series.Max((s) => (s as Series).Usi.SeriesNumber);

            }

            ModelClass.Containers.Add(new Series() { Usi=new Common.USI(this.Name,2021, series_max_index+1) });

        }
    }
}
