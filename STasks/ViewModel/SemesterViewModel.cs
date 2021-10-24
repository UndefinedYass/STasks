using STasks.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace STasks.ViewModel
{
    //NOTE: this serves as the HomeView data context
    public class SemesterViewModel: BaseDependencyProgViewModel, ITabContent
    {
        public SemesterViewModel(Semester model): base(model)
        {
            ClassesVMS = new ObservableCollection<ClassViewModel>(model.Classes.Select((c) => new ClassViewModel(c)));
            ClassCardsVMS = new ObservableCollection<ClassCardViewModel>(model.Classes.Select((c) => new ClassCardViewModel(c)));

            Name = model.Name;
            model.ProgressChanged += (s, e) =>
            {
                notif(nameof(TasksCountText));
            };
            this.ModelSemester = model;
            notif(nameof(TasksCountText));

        }

        public string Title
        {
            get { return "Home"; }
        }
       public bool IsSpecialHomeTab
        {
            get { return true; }
        }
        public bool IsDirty
        {
            get { return false; }
        }

        public SemesterViewModel():base(Common.Mock.GetDummySeries())
        {
            
			//desin time
            ClassesVMS = new ObservableCollection<ClassViewModel>(Common.Mock.GetDummySemester("S5").Classes.Select((c) => new ClassViewModel(c)));
            ClassCardsVMS = new ObservableCollection<ClassCardViewModel>(Common.Mock.GetDummySemester("S5").Classes.Select((c) => new ClassCardViewModel(c)));

            Name = "S5";
        }

        private string _Name;
        public string Name
        {
            set { _Name = value; notif(nameof(Name)); }
            get { return _Name; }
        }


        public string TasksCountText
        {
            get { return $"{ModelSemester.Progress.AllDescendantTasksCout - ModelSemester.Progress.AccomplishedTasksCount} Tasks to do in {(1)} month and {(4)} days."; }//todo: implemet date
        }

        public ObservableCollection<ClassViewModel> ClassesVMS { get; set; }
        public ObservableCollection<ClassCardViewModel> ClassCardsVMS { get; set; }
        public Semester ModelSemester { get; private set; }

        internal void update(Semester model)
        {
            ClassesVMS = new ObservableCollection<ClassViewModel>(model.Classes.Select((c) => new ClassViewModel(c)));
            ClassCardsVMS = new ObservableCollection<ClassCardViewModel>(model.Classes.Select((c) => new ClassCardViewModel(c)));

            Name = model.Name;
        }
    }
}
