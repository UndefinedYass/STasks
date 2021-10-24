using STasks.Common;
using STasks.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace STasks.Model
{
    
    public class Series : ProgressDependency
    {

        public event EventHandler<Exercise> ExerciseRemoved;//todo identify by guid
        public Series(Class class_par):base(class_par,false)
        {
            
        }
        public USI Usi { get; set; }

        public void InsertExercice(Exercise ex)
        {
            Exercises.Add(ex);
            
        }

        public void RemoveExercise(Exercise ex)
        {
            Exercises.Remove(ex);
            base.UnRegisterChild(ex);
            ExerciseRemoved?.Invoke(this, ex);

        }
        public IList<Exercise> Exercises { get; set; }

       
    }
}
