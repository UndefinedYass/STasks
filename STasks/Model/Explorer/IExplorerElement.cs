using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STasks.Model.Explorer
{
    /// <summary>
    /// consumed mainely by the UI (adding the right icons next to the eleement)
    /// </summary>
    public class ExplorerElementsAttributes
    {
        public bool IsSeries { get; set; }
        public bool IsClass { get; set; }
        public bool IsIncluded { get; set; }
        public bool IsMissing { get; set; }
        public bool IsWorkspace { get; set; }
        public string FileExtension { get; set; }
    }
    public enum ExplorerElementType { Folder,File}
    public interface IExplorerElement
    {
        ExplorerElementType Type { get; }
        IEnumerable<IExplorerElement> Children { get; }
        ExplorerElementsAttributes Attributes { get; }
        string Name { get; }

    }
}
