using STasks.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using STasks.Model.Explorer;


namespace STasks.Services
{
    public class ExplorerService
    {
        private static ExplorerService _Instance;
        public static ExplorerService Instance { get { if (_Instance == null) _Instance = new ExplorerService(); return _Instance; } }

        public ExplorerService()
        {
            WorkSpace = new ExplorerFolder(ConfigService.Instance.CurrentWorkspaceDirectory);
            WorkSpace.Resolve();
            WorkSpace.ResolveChildren();
        }

        public ExplorerFolder WorkSpace { get; set; } 



    }
}
