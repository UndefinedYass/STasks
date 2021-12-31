using STasks.Model.Bases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STasks.Model
{
    public class Exercise : STContainer
    {
        public Exercise()
        {
            Debug.WriteLine("Exercise ctor");
            this.guid = Guid.NewGuid();
        }

        public int ExerciseIndex { get; internal set; }

        //public override ObservableCollection<STDOMObject> Children { get; set; }


        public ObservableCollection<STBuildingBlock> Tasks
        {
            get
            {
                return Children;
            }
            set
            {
                Children = value;
                
            }
        }
        public string Title { get; internal set; }
    }
    public class Exercise2 : STContainer
    {
        public Exercise2() 
        {

        }


        public string Title { get; internal set; }
    }

    public class Task2 : STBuildingBlock
    {
        public Task2()
        {
           
        }


        public string Title { get; internal set; }
    }
}
