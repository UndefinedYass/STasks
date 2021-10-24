using STasks.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace STasks.Services
{
    public class DataService 
    {

        /// <summary>
        /// Loads object graph from file
        /// </summary>
        public static Semester LoadSTDoc(string pth)
        {

            var d= XDocument.Load(pth);
            var ddoc = d.Root;

            System.Diagnostics.Trace.Assert(ddoc.Name.LocalName == "STDoc");
            Trace.WriteLine(ddoc.Name);
            Trace.WriteLine(ddoc.FirstNode.NodeType);
            Trace.WriteLine((ddoc.FirstNode as XElement)?.Name );
            Semester res = null;
            return SemesterFromNode(ddoc.Element("SemesterDefinition"));
            /*try
            {
                res = SemesterFromNode(ddoc.Element("SemesterDefinition"));
            }
            catch (Exception err)
            {
                System.Diagnostics.Trace.WriteLine(err.Message);
                throw err;
            }
            return res;*/


        }


        private static TaskS TaskSFromNode(XElement taskElement, Exercise parent)
        {
            TaskS res = new TaskS(parent);
            res.Name = taskElement.Attribute("CustomName").Value;
            Trace.Assert(!string.IsNullOrEmpty(res.Name));
            Trace.WriteLine(res.Name);
            if (string.IsNullOrEmpty(res.Name)) throw new Exception("null");
            return res;//todo tasks should have index and name
        }

        private static Exercise ExerciseFromTaskNode(XElement exerciseXElement, Series parent)
        {
            
            Exercise res = new Exercise(parent);
            var x_coll= exerciseXElement.Element("TasksCollection");
            Trace.Assert(x_coll != null);
            var col = x_coll.Elements("Task").Select((x_task) => TaskSFromNode(x_task, res)).ToArray();
            res.Tasks= new List<TaskS>(col);
            res.Title = exerciseXElement.Attribute("CustomName").Value;
            return res;
        }
        private static Series SeriesFromNode(XElement SeriesXElement, Class parent)
        {
            Series res = new Series(parent);
            var x_coll = SeriesXElement.Element("ExercisesCollection");
            Trace.Assert(x_coll != null);
            var col = x_coll.Elements("ExerciseDefinition").Select((x_exer) => ExerciseFromTaskNode(x_exer, res)).ToArray();
            res.Usi= new Common.USI(parent.Name,2022, int.Parse(SeriesXElement.Attribute("USI").Value.Substring(3)));
            res.Exercises = new List<Exercise>(col);
            return res;
        }
        private static Class ClassFromNode(XElement ClassXElement, Semester parent)
        {
            Class res = new Class(parent);
            var x_coll = ClassXElement.Element("SeriesCollection");
            res.Name = ClassXElement.Attribute("Name").Value;
            Trace.Assert(!string.IsNullOrEmpty(res.Name));
            Trace.Assert(x_coll != null);
            var col = x_coll.Elements("SeriesDefinition").Select((x_Class) => SeriesFromNode(x_Class, res)).ToArray();
            res.Series = new List<Series>(col);
           
            res.ExamDate = DateTime.Parse(ClassXElement.Attribute("DeadlineDate").Value);
            return res;
        }
        private static Semester SemesterFromNode(XElement SemesterXElement)
        {
            Semester res = new Semester();
            var x_coll = SemesterXElement.Element("ClassesCollection");
            Trace.Assert(x_coll != null);
            res.Name = SemesterXElement.Attribute("Name").Value;
            Trace.Assert(!string.IsNullOrEmpty(res.Name));
            var col = x_coll.Elements("ClassDefinition").Select((x_Class) => ClassFromNode(x_Class, res)).ToArray();
            
            res.Classes = new List<Class>(col);
            res.IsAccomplished = true;
            res.IsAccomplished = false;
            
            return res;
        }

       

    }
}
