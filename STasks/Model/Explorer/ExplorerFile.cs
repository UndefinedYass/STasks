using STasks.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace STasks.Model.Explorer
{
    public class ExplorerFile : IExplorerElement
    {
        public ExplorerFile()
        {
            //d time
        }
        public ExplorerFile(string path)
        {
            AbsolutePath = path;
            Name = Path.GetFileName(path);
            Attributes.FileExtension = Path.GetExtension(path).Replace(".", "");
            if(Regex.IsMatch(Name.ToLower(), "td|serie|série"))
            {
                Attributes.IsSeries = true;
                Attributes.IsIncluded = true;
            }
        }
        public IEnumerable<IExplorerElement> Children { get; set; } = null;
        public ExplorerElementsAttributes Attributes { get; set; } = new ExplorerElementsAttributes();
        public string AbsolutePath { get; set; }
        public ExplorerElementType Type { get { return ExplorerElementType.File; } }
       public string Extension { get; set; }
        public string Name { get; set; }

        public ICommand OpenFileExternalCommand { get
            {
                return new MICommand(handleOpenFileExternalCommand);
            } }

        private void handleOpenFileExternalCommand()
        {
            Common.IOUtils.InvokeItemAsync(this.AbsolutePath);
        }
    }
}
