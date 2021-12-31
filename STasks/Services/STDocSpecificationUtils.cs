using STasks.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STasks.Services
{
    public class STDocSpecificationUtils
    {
        public enum XElements  { STDoc , SemesterDefinition , ClassDefinition, SeriesDefinition, ExcerciseDefinition, Task }
        public enum XCollectionWrapers {  ClassesCollection, SeriesCollection, ExcercisesCollection, TasksCollection }
        public static Dictionary<string, string[]> ElementsAttributes()
        {
            Dictionary<string, string[]> res = new Dictionary<string, string[]>();

           
            res["STDoc"] = new[] { "STDSVersion", };
            res["SemesterDefinition"] = new[] { "Name", };
            res["ClassDefinition"] = new[] { "Name", "DeadlineDate" };
            res["SeriesDefinition"] = new[] { "USI", };
            res["ExerciseDefinition"] = new[] { "ExerciseIndex", "CustomName" };
            res["Task"] = new[] { "TaskIndex", "CustomName" ,"ID"};


            return res;
        }
        

    }
}
