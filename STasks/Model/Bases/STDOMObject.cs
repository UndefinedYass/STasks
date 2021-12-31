using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace STasks.Model.Bases
{
    /// <summary>
    /// implement Observability, throu a basic set of events 
    /// ViewModels should make use of hese events to keep in sync with their models
    /// the goal is to avoid  unesecerry cpu pverhead of re constructing the whole lot when only few properties or collection elements change
    /// the model's properties chang is notified through the INotifyPropertyChanged
    /// while the Children list obeservability is hard coded using 3  events
    /// </summary>
    public abstract class STDOMObject : INotifyPropertyChanged


    {

        public STDOMObject()
        {

            Trace.WriteLine("STDOMObject ctor this.type: " + this.GetType().ToString());

        }

        public ProgressObject Progress { get; set; }

        //public ProgressObject Progress { get; internal set; }
        public STContainer Parent { get; set; }
        public STContainer GetRoot()
        {
            if (Parent == null) return this as STContainer;
            else return Parent.GetRoot();
        }

        #region INotif stuff
        internal void notif(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }


}
