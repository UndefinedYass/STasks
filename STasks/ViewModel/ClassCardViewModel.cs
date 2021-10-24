using System;
using STasks.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace STasks.ViewModel
{
    public class ClassCardViewModel  :BaseViewModel
    {
        public ClassCardViewModel()
        {
            //design-time
        }
        public ClassCardViewModel(int i)
        {

        }

        public ClassCardViewModel(Class c)
        {
            ModelClass = c;
            this.Title = c.Name;
            Prog = c.Progress;
            DeadlineDate = c.ExamDate;
            c.ProgressChanged += (s, e) => { Prog = c.Progress; };

        }

        private int _Remaining;
        public int Remaining
        {
            get { return Prog.AllDescendantTasksCout-Prog.AccomplishedTasksCount; }
        }


        private int _AllTasksCount;
        public int AllTasksCount
        {
            get { return Prog.AllDescendantTasksCout; }
        }


        private int _AccomplishedTasksCount;
        public int AccomplishedTasksCount
        {
            get { return Prog.AccomplishedTasksCount; }
        }


        private string _Title;
        public string Title
        {
            set { _Title = value; notif(nameof(Title)); }
            get { return _Title; }
        }


        private DiscretProgress _Prog;
        public DiscretProgress Prog
        {
            set { _Prog = value; notif(nameof(Prog)); notif(nameof(Remaining));
                notif(nameof(AllTasksCount)); notif(nameof(AccomplishedTasksCount));
                notif(nameof(ProgressColor)); notif(nameof(IsComplete)); notif(nameof(IsProgressVisible));
            }
            get { return _Prog; }
        }


        private DateTime _DeadlineDate;
        private Class c;

        public DateTime DeadlineDate
        {
            set { _DeadlineDate = value; notif(nameof(DeadlineDate)); }
            get { return _DeadlineDate; }
        }


        public System.Windows.Media.Brush ProgressColor
        {
            get { return new SolidColorBrush(Common.ColorUtils.GreenRedScaleDiscret(Prog.Progress)); }
        }

        public bool IsComplete
        {
            get { return Prog.Progress==1; }
        }

        /// <summary>
        /// Progress bar and text should not be visible when it's 0% or 100% or NaN
        /// </summary>
        public bool IsProgressVisible
        {
            get { var p = Prog.Progress; return p !=0 && p!= float.NaN && p < 1; }
        }




        public ICommand OpenCommand { get {
                return new Common.MICommand(() => {
                    var mvm = ((App.Current.MainWindow as MainWindow).DataContext as MainViewModel);
                    mvm.OpenTabAndSelect(this.ModelClass);
                });
            } }

        public Class ModelClass { get; private set; }
    }
}
