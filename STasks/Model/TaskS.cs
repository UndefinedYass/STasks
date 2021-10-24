using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STasks.Model
{
    public class TaskS: ProgressDependency
    {
        public TaskS(Exercise parent) : base(parent,false)
        {
            IsBuildingBlock = true;
            Progress = new DiscretProgress(1, 0);
        }
       
        public string Name { get; set; }
        public int TaskIndex { get; set; }

    }
}
