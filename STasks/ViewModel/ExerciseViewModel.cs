using STasks.Common;
using STasks.Model;
using STasks.Model.Bases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace STasks.ViewModel
{

    public class AddMultipleTasksDialogViewModel: BaseViewModel
    {

        public AddMultipleTasksDialogViewModel(ExerciseViewModel relative)
        {
            RelativeExerciseViewModel = relative;
        }

        private ExerciseViewModel RelativeExerciseViewModel;
       



        private string _Count;
        public string Count
        {
            set { _Count = value; notif(nameof(Count)); }
            get { return _Count; }
        }

        public ICommand AddCommand { get { return new MICommand(handleAddCommand); } }

        private void handleAddCommand()
        {
            if (string.IsNullOrWhiteSpace( Count)==false )
                if(RelativeExerciseViewModel!=null)
                RelativeExerciseViewModel.AddTasksCommand.Execute(Count);

            RelativeExerciseViewModel.IsDialogOpen = false;
        }
    }
    public class ExerciseViewModel : BaseProgressObjectViewModel, IRenamingTextBoxViewModel
    {
        public ExerciseViewModel(Exercise ex ):base(ex)
        {
            ModelExercise = ex;
            TasksVMS = new ObservableCollection<TaskViewModel>(ex.Tasks.Select((tsk) => new TaskViewModel((TaskS) tsk)));
            Title = ex.Title;
            RenamingName = Title;
            IsExpanded = true;

            AddMultipleTasksDialogViewModelInstance = new AddMultipleTasksDialogViewModel(this);
            
        }
       
        public ExerciseViewModel():base( Mock.GetDummyExercise("d time exrcie"))
        {
            TasksVMS = new ObservableCollection<TaskViewModel>(Common.Mock. GetDummyExercise().Tasks.Select((tsk) => new TaskViewModel((TaskS)tsk)));
            Title = "EX2: D-TIME TITLE";
            //d time
        }



        private AddMultipleTasksDialogViewModel _AddMultipleTasksDialogViewModelInstance;
        public AddMultipleTasksDialogViewModel AddMultipleTasksDialogViewModelInstance
        {
            set { _AddMultipleTasksDialogViewModelInstance = value; notif(nameof(AddMultipleTasksDialogViewModelInstance)); }
            get { return _AddMultipleTasksDialogViewModelInstance; }
        }



        private string _Title;
        public string Title
        {
            set { _Title = value; notif(nameof(Title)); }
            get { return _Title; }
        }

        internal override void handleChildAdded(STBuildingBlock newBuildingBlock)
        {
            TaskViewModel new_tvm = new TaskViewModel((TaskS)newBuildingBlock);
            new_tvm.IsRenaming = true;
            TasksVMS.Add(new_tvm);
        }
        internal override void handleChilRemoved(STBuildingBlock removedBuildingBlock)
        {
            TaskS rem = removedBuildingBlock as TaskS;
            TaskViewModel removedOne = TasksVMS.FirstOrDefault((cc) => cc.ModelTask == rem);
            TasksVMS.Remove(removedOne);
        }
        internal override void handleContainersCollectionReset()
        {
            TasksVMS = new ObservableCollection<TaskViewModel>(Common.Mock.GetDummyExercise().Tasks.Select((tsk) => new TaskViewModel((TaskS)tsk)));
            notif(nameof(TasksVMS));
        }

        private bool _IsExpanded;
        public bool IsExpanded
        {
            set { _IsExpanded = value; notif(nameof(IsExpanded)); }
            get { return _IsExpanded; }
        }

       

        public ICommand ClearTasksCommand
        {
            get { return new Common.MICommand(HandleClearTasksCommand); }
        }

        private void HandleClearTasksCommand()
        {
            ModelExercise.Tasks.Clear();
        }

        public ICommand AddTaskCommand
        {
            get { return new Common.MICommand(HandleAddTaskCommand); }
        }


        private string GetSmartNewTaskTitle()
        {
            string smartTitle = "Task 1";
            TaskS lastelem = ModelExercise.Tasks.LastOrDefault() as TaskS;
            if (lastelem == null) return smartTitle;
            else
            {
                var m = Regex.Match(lastelem.Name, @"(.*)(\d+)");
                if (m.Success)
                {
                    int lst_nm = int.Parse(m.Groups[2].Value);
                    smartTitle = m.Groups[1].Value + (lst_nm + 1).ToString();
                }
               
            }

            return smartTitle;
        }



        private void HandleAddTaskCommand()
        {
            var new_t = new TaskS() { Name = GetSmartNewTaskTitle(), Guid=Guid.NewGuid() };
            ModelExercise.Tasks.Add(new_t);
            // ExercisesVMS.Add(new ExerciseViewModel(new_ex) { IsRenaming = true });

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
            
            (ModelExercise.Parent as Series).Containers.Remove(ModelExercise);
            
        }


        public ICommand OpenDialogCommand { get { return new Common.MICommand<object>(handleOpenDialogCommand); } }

        private void handleOpenDialogCommand(object fe)
        {

            CurrentDialogContent = fe;
            IsDialogOpen = true;
        }
        //AddTasksCommand

        public ICommand AddTasksCommand
        {
            get { return new Common.MICommand<string>(HandleAddTasksCommand); }
        }

        private void HandleAddTasksCommand(string obj)
        {
            int count;
            if (int.TryParse(obj,out count))
            {
                for (int i = 0; i < count; i++)
                {
                    HandleAddTaskCommand();
                }
            }
        }

        private object _CurrentDialogContent;
        public object CurrentDialogContent
        {
            set { _CurrentDialogContent = value; notif(nameof(CurrentDialogContent)); }
            get { return _CurrentDialogContent; }
        }



        public void TextBoxLostFocusOrEnterKeyCallBack(bool HasLostFocus)
        {
           
            if (ValidateExcerciseName(RenamingName))
            {
                IsRenaming = false;
                ModelExercise.Title = RenamingName;
                Title = RenamingName;
            }
        }


        private bool ValidateExcerciseName(string renamingName)
        {
            return string.IsNullOrWhiteSpace(renamingName) == false;
        }



        

        private bool _IsDialogOpen;
        public bool IsDialogOpen
        {
            set { _IsDialogOpen = value; notif(nameof(IsDialogOpen)); }
            get { return _IsDialogOpen; }
        }


        public ObservableCollection<TaskViewModel> TasksVMS { get; set; }
        public Exercise ModelExercise { get; private set; }
    }
}
