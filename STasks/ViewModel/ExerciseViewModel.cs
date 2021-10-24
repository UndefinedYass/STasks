using STasks.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace STasks.ViewModel
{
    public class ExerciseViewModel : BaseDependencyProgViewModel, IRenamingTextBoxViewModel
    {
        public ExerciseViewModel(Exercise ex ):base(ex)
        {
            ModelExercise = ex;
            TasksVMS = new ObservableCollection<TaskViewModel>(ex.Tasks.Select((tsk) => new TaskViewModel(tsk)));
            Title = ex.Title;
            RenamingName = Title;
            IsExpanded = true;
        }
       
        public ExerciseViewModel():base( new Exercise(new Series(null)))
        {
            TasksVMS = new ObservableCollection<TaskViewModel>(Common.Mock. dummyEx().Tasks.Select((tsk) => new TaskViewModel(tsk)));
            Title = "EX2: D-TIME TITLE";
            //d time
        }


        private string _Title;
        public string Title
        {
            set { _Title = value; notif(nameof(Title)); }
            get { return _Title; }
        }


        private bool _IsExpanded;
        public bool IsExpanded
        {
            set { _IsExpanded = value; notif(nameof(IsExpanded)); }
            get { return _IsExpanded; }
        }






        private bool _IsRenaming;
        public bool IsRenaming
        {
            set { _IsRenaming = value; notif(nameof(IsRenaming)); }
            get { return _IsRenaming; }
        }


        private string _RenamingName;
        public string RenamingName
        {
            set { _RenamingName = value; notif(nameof(RenamingName)); }
            get { return _RenamingName; }
        }



        public ICommand RenameCommand { get { return new Common.MICommand(handleRenameCommand); } }

        private void handleRenameCommand()
        {
            IsRenaming = true;
        }

        public ICommand DeleteCommand { get { return new Common.MICommand(handleDeleteCommand); } }

        private void handleDeleteCommand()
        {
            (ModelExercise.Parent as Series).RemoveExercise(ModelExercise);
            
        }

        public void TextBoxLostFocusOrEnterKeyCallBack(bool HasLostFocus)
        {
           
            if (ValidateExcerciseName(RenamingName))
            {
                IsRenaming = false;
                Title = RenamingName;
            }
        }


        private bool ValidateExcerciseName(string renamingName)
        {
            return string.IsNullOrWhiteSpace(renamingName) == false;
        }

        public ObservableCollection<TaskViewModel> TasksVMS { get; set; }
        public Exercise ModelExercise { get; private set; }
    }
}
