using STasks.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Windows.Input;
using STasks.Common;
using System.Windows.Controls;
using System.Collections;
using System.Diagnostics;
using STasks.Model.Bases;

namespace STasks.ViewModel
{
    //NOTE: this serves as the HomeView data context
    public class SemesterViewModel: BaseProgressObjectViewModel, ITabContent
    {
        public SemesterViewModel(Semester model): base(model)
        {

            ClassesVMS = new ObservableCollection<ClassViewModel>(model.Classes.Select((c) => new ClassViewModel(c as Class)));
            ClassCardsVMS = new ObservableCollection<ClassCardViewModel>(model.Classes.Select((c) => new ClassCardViewModel(c as Class)));

            Name = model.Name;
            model.Progress.PropertyChanged += (s, e) =>
            {
                notif(nameof(TasksCountText));
            };
            this.ModelSemester = model;
            notif(nameof(TasksCountText));

        }

        //DeleteSelectedClasses

        public ICommand DeleteSelectedClassesCommand
        {
            get { return new MICommand<IList>(HandleDeleteSelectedClasses); }
        }

        private void HandleDeleteSelectedClasses(IList selection)
        {
            var to_be_deleted = new Class[selection.Count];
            for (int i = 0; i < selection.Count; i++)
            {
                to_be_deleted[i] = (selection[i] as ClassCardViewModel).ModelClass;
            }
            foreach (Class item in to_be_deleted)
            {
                
                ModelSemester.Classes.Remove(item);
            }
           
        }

        public ICommand AddClassCommand
        {
            get { return new MICommand(HandleAddClassCommand); }
        }

        private void HandleAddClassCommand()
        {
            Class new_class = new Class() { Name = "New Class" };
            
            ModelSemester.Containers.Add(new_class);
            Trace.WriteLine("added class to Semester's containers collection");
        }

        internal override void handleContainerAdded(STContainer newContainer)
        {
            ClassCardsVMS.Add( new ClassCardViewModel(newContainer as Class));
            ClassesVMS.Add(new ClassViewModel(newContainer as Class));

        }
        internal override void handleContainerRemoved(STContainer removedContainer)
        {
            Class cls = removedContainer as Class;
            ClassCardViewModel removedOne = ClassCardsVMS.FirstOrDefault((cc) => cc.ModelClass == cls);
            ClassCardsVMS.Remove(removedOne);
            ClassViewModel cvm = ClassesVMS.FirstOrDefault((cvm_) => cvm_.ModelClass == cls);
            if(cvm!=null)
            ClassesVMS.Remove(cvm);
            
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
        static Semester d_time_s;
        public SemesterViewModel():base((d_time_s = Common.Mock.GetDummySemester("Design-time Semester")))
        {
            
			//desin time
            ClassesVMS = new ObservableCollection<ClassViewModel>(d_time_s.Classes.Select((c) => new ClassViewModel(c as Class)));
            ClassCardsVMS = new ObservableCollection<ClassCardViewModel>(d_time_s.Classes.Select((c) => new ClassCardViewModel(c as Class)));

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
            get {
                DateTime duedate = ClassCardsVMS.Min(c => c.ModelClass.ExamDate);
                var remainingTime = duedate - DateTime.Now;

                return $"{ModelSemester.Progress.TotalCount - ModelSemester.Progress.CompletedCount} Tasks to do in {(remainingTime.Days)} days and {(remainingTime.Hours)} hours.";

            }//todo: implemet date
        }

        public ObservableCollection<ClassViewModel> ClassesVMS { get; set; }
        public ObservableCollection<ClassCardViewModel> ClassCardsVMS { get; set; }
        public Semester ModelSemester { get; private set; }

        
    }
}
