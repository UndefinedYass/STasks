using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STasks.Model;
using STasks.ViewModel;

namespace STasks.Common
{
    public class Mock
    {
        public static Exercise GetDummyExercise(string name ="Exercise",double completionBias=double.NaN)
        {
            Exercise ex = new Exercise();
            int s_count = MathUtils.RandInt(1, 5);
            for (int i = 0; i < s_count; i++)
            {
                TaskS t = new TaskS();
                bool t_value = double.IsNaN(completionBias )? MathUtils.RndBool() : MathUtils.RndBasedBool(completionBias);

                t.Name = "Task " + (i + 1).ToString();
                ex.Tasks.Add(t);
                t.Progress.RequireCompletion(t_value);
            }

            return ex;
            //SeriesModel.Exercises = new[] { ex1, ex2 };
        }

        internal static Semester GetDummySemester(string n="dummy semester", double completionBias = double.NaN)
        {
            Semester s = new Semester();
            s.Name = n;
            
            var clss = new[] {
                GetDummyClass("Electro",1 ),
                GetDummyClass("Magnetism",0.7),
                GetDummyClass("Math",0.5),
                GetDummyClass("Physique Quantique",0.1),
                GetDummyClass("ELECTRONIQUE ANALOGIQUE",0.99),
            };
            foreach (var item in clss)
            {
                s.Classes.Add(item);
            }
            return s;
        }

        internal static Class GetDummyClass(string name="dmmy", double completionBias = double.NaN)
        {
            Class cl = new Class();
            cl.Name = name;

            int srs_count = MathUtils.RandInt(2, 4);
            for (int i = 0; i < srs_count; i++)
            {
                Series s = GetDummySeries(i + 1, completionBias);
                cl.Series.Add(s);
            }
            return cl;
        }

        public static Series GetDummySeries(int ser_num = 3, double completionBias = double.NaN, string class_name="unk")
        {
            var s = new Series();
            s.Usi = new USI(class_name, 2021, ser_num);

            int exes_count = MathUtils.RandInt(3, 7);
            for (int i = 0; i < exes_count; i++)
            {
                Exercise ex = GetDummyExercise($"Exercise {i + 1}", completionBias);
                ex.Title = "Exercise " + (i + 1).ToString();
                s.Exercises.Add(ex);//it was s.Exercises.Add(s); and i hit a stackoverflow that drove me nuts 24-oct-2021
            }

            return s;
        }
    }
}
