using STasks.Model.Bases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STasks.Model
{
    public class TaskS: STBuildingBlock
    {
        public TaskS() 
        {
            Debug.WriteLine("TaskS ctor");
        }
       
        public string Name { get; set; }
        public DateTime CompletionDate { get; set; }
        public int TaskIndex { get; set; }
        public Guid Guid { get; set; }

    }
}
