using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STasks.Model.Bases;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Linq;

namespace STasks.Model.Bases
{

    public abstract class STBuildingBlock : STDOMObject
    {
        public STBuildingBlock()
        {
            Progress = new SimpleProgress((STBuildingBlock)this);

        }
        public new SimpleProgress Progress
        {
            get { return (SimpleProgress)base.Progress; }
            set { base.Progress = value; }
        }

    }

}
