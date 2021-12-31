using System;
using STasks.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using STasks.Common;
using STasks.Model.Bases;

namespace STasks.ViewModel
{
    public class ClassCardViewModel  :BaseViewModel, IRenamingTextBoxViewModel
    {
        public ClassCardViewModel()
        {
            //design-time
        }
       

        public ClassCardViewModel(Class c)
        {
            ModelClass = c;
            
            Prog = c.Progress;
            DeadlineDate = c.ExamDate;
            c.Progress.PropertyChanged += (s, e)=>{
                //todo notif
                notif(nameof(IsDoneIconVisible));
                notif(nameof(Remaining));
                notif(nameof(AllTasksCount));
                notif(nameof(AccomplishedTasksCount));
                notif(nameof(ProgressColor));
                notif(nameof(IsProgressVisible));

            };
            ModelClass.PropertyChanged += (s, e) =>
            {
                notif(nameof(Title));
                notif(nameof(DeadlineDate));
            };
            EditingDate = ModelClass.ExamDate;
            EditingNotes = ModelClass.Notes;
            

        }

        
        public int Remaining
        {
            get { return Prog.TotalCount-Prog.CompletedCount; }
        }


        
        public int AllTasksCount
        {
            get { return Prog.TotalCount; }
        }


        
        public int AccomplishedTasksCount
        {
            get { return Prog.CompletedCount; }
        }


        public string Title
        {
          
            get { return  ModelClass.Name; }
        }


        private ProgressObject _Prog;
        public ProgressObject Prog
        {
            set { _Prog = value; notif(nameof(Prog)); notif(nameof(Remaining));
                notif(nameof(AllTasksCount)); notif(nameof(AccomplishedTasksCount));
                notif(nameof(ProgressColor)); notif(nameof(IsComplete)); notif(nameof(IsProgressVisible));
            }
            get { return _Prog; }
        }


        private DateTime _DeadlineDate;
        

        public DateTime DeadlineDate
        {
            set { _DeadlineDate = value; notif(nameof(DeadlineDate)); }
            get { return _DeadlineDate; }
        }


        public System.Windows.Media.Brush ProgressColor
        {
            get { return new SolidColorBrush(Common.ColorUtils.GreenRedScaleDiscret(Prog.Ratio)); }
        }

        //todo remove useless property
        public bool IsComplete
        {
            get { return Prog.IsComplete; }
        }

        /// <summary>
        /// according to UX specifications this should be IsComplete and IsDetermined
        /// </summary>
        public bool IsDoneIconVisible
        {
            get { return  Prog.IsComplete && Prog.IsDetermined; }
        }



        /// <summary>
        /// UX Specs: ture when IsDetermined And not IsComplete and percent is not 0
        /// </summary>
        public bool IsProgressVisible
        {
            get { return Prog.IsDetermined && Prog.Percent!=0 &&  !Prog.IsComplete; }
        }




        public ICommand OpenCommand { get {
                return new Common.MICommand(() => {
                    var mvm = ((App.Current.MainWindow as MainWindow).DataContext as MainViewModel);
                    mvm.OpenTabAndSelect(this.ModelClass);
                });
            } }

        public Class ModelClass { get; private set; }


        private bool _IsRenaming;
        public bool IsRenaming
        {
            set { _IsRenaming = value; notif(nameof(IsRenaming)); }
            get { return _IsRenaming; }
        }



        public ICommand RenameCommand
        {
            get
            {
                return new MICommand(handleRenameCommand);
            }
        }

        private void handleRenameCommand()
        {
            RenamingName = ModelClass.Name;
            IsRenaming = true;
        }


        private string _RenamingName;
        public string RenamingName
        {
            set { _RenamingName = value; notif(nameof(RenamingName)); }
            get { return _RenamingName; }
        }

        //OpenPropertiesCommand

        public ICommand OpenPropertiesCommand
        {
            get { return new MICommand(handleOpenPropertiesCommand); }
        }
        public ICommand SavePropertiesCommand
        {
            get { return new MICommand(handleSavePropertiesCommand); }
        }


        private DateTime _EditingDate;
        public DateTime EditingDate
        {
            set { _EditingDate = value; notif(nameof(EditingDate)); }
            get { return _EditingDate; }
        }


        private string _EditingNotes;
        public string EditingNotes
        {
            set { _EditingNotes = value; notif(nameof(EditingNotes)); }
            get { return _EditingNotes; }
        }



        private void handleSavePropertiesCommand()
        {
            ModelClass.ExamDate = EditingDate;
            ModelClass.Notes = EditingNotes;
            DeadlineDate = ModelClass.ExamDate;
            notif(nameof(Title));
            notif(nameof(DeadlineDate));

        }

        private void handleOpenPropertiesCommand()
        {
            View.ClassPropertiesWindow cpw = new View.ClassPropertiesWindow();
            cpw.DataContext = this;
            cpw.ShowDialog();
        }

        public ICommand RequireCompletionCommand
        {
            get { return new MICommand<bool>(handleRequireCompletionCommand, (val) => ModelClass.Progress.IsDetermined); }
        }

        private void handleRequireCompletionCommand(bool value)
        {
            ModelClass.Progress. RequireCompletion(value);
        }


        public void TextBoxLostFocusOrEnterKeyCallBack(bool HasLostFocus)
        {
            IsRenaming = false;
            if (validateRenamingName(RenamingName))
            {
                ModelClass.Name = RenamingName;
                //notif(nameof(Title));

            }
            else
            {
                //todo feedback name is invalid
            }
            
        }

        private bool validateRenamingName(string renamingName)
        {
            return (string.IsNullOrWhiteSpace(RenamingName)) == false && (renamingName.Contains(".")==false);
        }
    }
}
