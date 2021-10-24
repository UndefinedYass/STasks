using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace STasks.ViewModel
{
    interface IRenamingTextBoxViewModel
    {
        void TextBoxLostFocusOrEnterKeyCallBack(bool HasLostFocus);
        ICommand RenameCommand { get; }
        string RenamingName { get; }
    }
}
