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
    public class Class : STContainer
    {

        public Class() 
        {
            Debug.WriteLine("Class ctor");
        }


      
        

        private string _Name= string.Empty;
        public string Name
        {
            set { _Name = value;
                notif(nameof(Name));
                Trace.WriteLine("nam set");
            }
            get { return _Name; }
        }





        //todo class names need validation e.g cannot contain dots as that interfers with the URS ruls
        public DateTime ExamDate { get; set; }
        public ObservableCollection<STContainer> Series { get { return Containers; } set { Containers = value; } }

        public string Notes { get; internal set; }
    }
}
