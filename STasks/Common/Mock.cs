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
        public static Exercise dummyEx()
        {
            var SeriesModel = new Series(null);
            var ex1 = new Exercise(SeriesModel);
            var ex2 = new Exercise(SeriesModel);

            Model.TaskS t11 = new TaskS(ex1);
            Model.TaskS t12 = new TaskS(ex1);
            Model.TaskS t21 = new TaskS(ex2);
            Model.TaskS t22 = new TaskS(ex2);

            ex1.Tasks = new[] { t11, t12 };
            ex2.Tasks = new[] { t21, t22 };
            return ex1;
            //SeriesModel.Exercises = new[] { ex1, ex2 };
        }

        internal static Semester GetDummySemester(string n="dummy semester", DiscretProgress? Prog = null)
        {
            Semester s = new Semester();
            s.Name = n;
            s.Progress = Prog.HasValue? Prog.Value : DiscretProgress.zero;
            s.Classes = new[] {
                GetDummyClass("Electro", s, new DiscretProgress(145,54)),
                GetDummyClass("Magnetism",s, new DiscretProgress(266,266)),
                GetDummyClass("Math",s, new DiscretProgress(118,0)),
                GetDummyClass("Physique Quantique",s, new DiscretProgress(80,40)),
                GetDummyClass("ELECTRONIQUE ANALOGIQUE",s, new DiscretProgress(11,11)),
            };
            return s;
        }

        internal static Class GetDummyClass(string name="dmmy", Semester class_par = null, DiscretProgress? Prog = null)
        {
            Class cl = new Class(class_par);
            cl.Name = name;
            cl.Progress = Prog.HasValue ? Prog.Value : DiscretProgress.zero;
            cl.Series = new[] { GetDummySeries(1,cl), GetDummySeries(2,cl), GetDummySeries(3,cl) };
            return cl;
        }

        public static Series GetDummySeries(int ser_num = 3, Class class_par=null)
        {
            var SeriesModel = new Series(class_par);
            SeriesModel.Usi = new USI(class_par.Name, 2021, ser_num);
            var ex1 = new Exercise(SeriesModel);
            var ex2 = new Exercise(SeriesModel);

            Model.TaskS t11 = new TaskS(ex1);
            Model.TaskS t12 = new TaskS(ex1);
            Model.TaskS t21 = new TaskS(ex2);
            Model.TaskS t22 = new TaskS(ex2);

            ex1.Tasks = new[] { t11, t12 };
            ex2.Tasks = new[] { t21, t22 };
            SeriesModel.Exercises = new[] { ex1, ex2 };
            return SeriesModel;
        }
    }
}
