using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STasks.Model
{
    public class Exercise : ProgressDependency
    {
        public Exercise(Series parent):base(parent,false)
        {

        }

        //public override ObservableCollection<STDOMObject> Children { get; set; }
       

        public IList<Model.TaskS> Tasks { get; set; }
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
