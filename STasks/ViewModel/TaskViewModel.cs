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
    public class TaskViewModel : BaseProgressObjectViewModel, IRenamingTextBoxViewModel
    {
        public ICommand RenameCommand
        {
            get
            {
                return new Common.MICommand(handleRenameCommand);
            }
        }

        private void handleRenameCommand()
        {
            IsRenaming = true;
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

        public TaskS ModelTask { get; private set; }

        public TaskViewModel(STasks.Model.TaskS modelTask): base(modelTask)
        {

            this.ModelTask = modelTask;
            this.RenamingName = ModelTask.Name;
            notif(nameof(Title));
        }



        public string Title
        {
            get { return ModelTask.Name; }
        }


        public TaskViewModel():base(null)
        {
            //d time
        }

        public void TextBoxLostFocusOrEnterKeyCallBack(bool HasLostFocus)
        {
            IsRenaming = false;
            ModelTask.Name = RenamingName;
            notif(nameof(Title));
        }
    }
}
