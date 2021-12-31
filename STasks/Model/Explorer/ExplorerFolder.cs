using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using STasks.Common;

namespace STasks.Model.Explorer
{
    public class ExplorerFolder : IExplorerElement, IExpandedCommandEvent
    {
        public ExplorerFolder()
        {
            //d time
        }
        public ExplorerFolder(string path)
        {
            AbsolutePath = path;
            Name =new DirectoryInfo(path).Name;
            if (Name == "PHYSIQUE STATISTIQUE"|| Name == "PHYSIQUE QUANTIQUE"|| Name == "CHIMIE ORGANIQUE")
            {
                Attributes.IsClass = true;
                Attributes.IsIncluded = true;
            }
        }

        public ExplorerElementsAttributes Attributes { get; set; } = new ExplorerElementsAttributes();
        public string Name { get; set; }
        public string AbsolutePath { get; set; }
        public ExplorerElementType Type { get { return ExplorerElementType.Folder; } }
      
        public ObservableCollection<ExplorerFolder> Folders { get; set; }
        public ObservableCollection<ExplorerFile> Files { get; set; }
        public IEnumerable<IExplorerElement> Children { get; set; } = null;
        public bool IsResolved { get; set; }
        public bool IsChildrenResolved { get; set; }


        /// <summary>
        /// scans the directory and populates the internal elements, returning the total count of elements
        /// </summary>
        public int Resolve()
        {
            int count = 0;
            count += (Folders = new ObservableCollection<ExplorerFolder>(Directory.GetDirectories(AbsolutePath).Select((d) => new ExplorerFolder(d)))).Count;
            count += (Files = new ObservableCollection<ExplorerFile>(Directory.GetFiles(AbsolutePath).Select((f) => new ExplorerFile(f)))).Count;
            
            Children = (Folders as IEnumerable<IExplorerElement>).Concat(Files);
            IsResolved = true;
            return count;
                
        }

        /// <summary>
        /// scans the sub directory 
        /// </summary>
        public void ResolveChildren()
        {
            foreach (var item in Folders)
            {
                item.Resolve();
            }
            IsChildrenResolved = true;

        }


        public ICommand RefreshCommand { get { return new MICommand(HandleRefreshCommand); } }

        private void HandleRefreshCommand()
        {
            
        }

        public ICommand ExpandedCommandEvent { get { return new MICommand(HandleExpandedCommandEvent); } }

        private void HandleExpandedCommandEvent()
        {
            if (IsChildrenResolved == false)
            {
                ResolveChildren();
            }
        }
    }


    public interface IExpandedCommandEvent
    {
        ICommand ExpandedCommandEvent { get ;  }

    }

}
