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
    /// <summary>
    /// smalest block is task, a semester consists of multimple Classes
    /// a Class (might be renamed SClass later) consists of multiple Series
    /// a series have exercises which have a set of tasks. whch are the smalles elements to accoumplish
    /// this graph is supposted to be defined by the user, and flexibly altered any time later
    /// 
    /// there should be some sort of binding between Series objects and fileSystem documents (e.g PDF files)
    /// where a file path is kept and maybe a hash value as well for intergrty
    /// the process of creating a series may look like importinga file
    /// </summary>


    public class Semester : STContainer
    {
        public Semester()
        {
            Debug.WriteLine("Semester ctor");
        }
        public string Name { get; set; }
        public ObservableCollection<STContainer> Classes { get { return Containers; } set { Containers = value; } }
    }
}
