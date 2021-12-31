using STasks.Common;
using STasks.Model.Bases;
using STasks.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace STasks.Model
{
    
    public class Series : STContainer
    {

        public Series()
        {
            Debug.WriteLine("series ctor");
        }
        public USI Usi { get; set; }

        public void InsertExercice(Exercise ex)
        {
            Exercises.Add(ex);
            
        }

        public void RemoveExercise(Exercise ex)
        {
           

        }
        public ObservableCollection<STContainer> Exercises { get { return Containers; } set { Containers = value; } }

       
    }
}
