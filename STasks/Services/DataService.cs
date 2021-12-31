using STasks.Model;
using STasks.Model.Bases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace STasks.Services
{
    public class DataService 
    {



        /// <summary>
        /// creates a userData oject from the semester and saves it
        /// 
        /// </summary>
        public static void SaveSTUserData(Semester semester, string path)
        {
            STUserData ud = MakeUserDataFromSemester(semester);
            ud.Save(path);
        }

        private static STUserData MakeUserDataFromSemester(Semester semester)
        {
            STUserData res = new STUserData();
            res.SemesterName = semester.Name;
            res.setBuildingBlocksData(new Dictionary<Guid, BuildingBlockData>());
            Dictionary<Guid, BuildingBlockData> dict = new Dictionary<Guid, BuildingBlockData>();
            foreach (TaskS item in semester.GetAllChildAndDescendantBuildingBlocks())
            {

                dict.Add(item.Guid, new BuildingBlockData() { Value= item.Progress.IsComplete, CompletionDate= (item as TaskS).CompletionDate, Guid = item.Guid });
                if (item.Progress.IsComplete) Trace.WriteLine("one was tru");
            }
            
            res.setBuildingBlocksData(dict);
            return res;
        }

        /// <summary>
        /// saves the STDoc , 
        /// Note ths soest save or creat user data file, use SaveUserData(semester s) static function 
        /// </summary>
        /// <param name="semester"></param>
        /// <param name="stdoc_path"></param>
        public static void SaveSTDoc(Semester semester, string stdoc_path)
        {
            //methodes or variables prefixed wth x refer to an xelement type representing ones of the elements documnted in dpecification
            XDocument xdoc = new XDocument();
            XElement XSTDoc = GetXSTDoc();
            XSTDoc.Add(XDfinitionFromSTContaner(semester));
            xdoc.Add(XSTDoc);
            xdoc.Save(stdoc_path);
        }

        private static XElement XDfinitionFromSTContaner(STContainer STContainer)
        {
            //#Make the XElement (<FooDefinition>) and populate it's attribs
            XElement XElem = new XElement(GetXElementTagName(STContainer));
            PopulateAttributes(XElem, STContainer);
            //#Add to the XElement a (<TasksCollection>) that is populated with cildren buildng blocks

            //todo building blocks children currentely not supported on types other than exercise
            XElement XTasksCollection = new XElement("TasksCollection");
            foreach (STBuildingBlock item in STContainer.Children)
            {
                XElement Xtask = new XElement("Task");
                PopulateAttributes(Xtask, item as TaskS);
                XTasksCollection.Add(Xtask);
            }
            XElem.Add(XTasksCollection);


            //#Add to the XElement a (<FooCollection>) that is populated with children containers 
            XElement XFooCollection = new XElement(GetCollectionTagName(STContainer));
            foreach (STContainer item in STContainer.Containers)
            {
                XFooCollection.Add(XDfinitionFromSTContaner(item));
            }
            XElem.Add(XFooCollection);
            return XElem;
        }

        /// <summary>
        /// helper method; knows what attribs should look for for every type base on the specification
        /// </summary>
        private static void PopulateAttributes(XElement xObj, STDOMObject domObj)
        {
            if(domObj is TaskS)
            {
                xObj.SetAttributeValue("CustomName", ((TaskS)domObj).Name);
                xObj.SetAttributeValue("TaskIndex", ((TaskS)domObj).TaskIndex);
                xObj.SetAttributeValue("ID", ((TaskS)domObj).Guid);
            }
            if (domObj is Exercise)
            {
                xObj.SetAttributeValue("CustomName", ((Exercise)domObj).Title);
                xObj.SetAttributeValue("ExerciseIndex", ((Exercise)domObj).ExerciseIndex);
            }
            if (domObj is Series)
            {
                xObj.SetAttributeValue("USI", ((Series)domObj).Usi);
            }
            if (domObj is Class)
            {
                xObj.SetAttributeValue("Name", ((Class)domObj).Name);
                xObj.SetAttributeValue("DeadlineDate", ((Class)domObj).ExamDate);
            }
            if (domObj is Semester)
            {
                xObj.SetAttributeValue("Name", ((Semester)domObj).Name);
            }
            return;
        }

        private static XElement GetXSTDoc()
        {
            XElement stdoc = new XElement("STDoc");
            stdoc.SetAttributeValue("STDSVersion", 1);
            return stdoc;
        }



        

        public static string GetXElementTagName(STDOMObject obj)
        {
            string tagName = "";
            if(obj is STContainer)
            {
                tagName= obj.GetType().ToString().Replace("STasks.Model.", "") + "Definition";
            }
            else if(obj is STBuildingBlock)
            {
                if (obj is TaskS) tagName= "Task";
            }
           
            //Semester : SemesterDefinition
            //Class : ClassDefinition
            //Series : SeriesDefinition
            //Exercise : ExerciseDefinition
            //TaskS : Task

            return tagName;
        }
        public static string GetCollectionTagName(STContainer obj)
        {
            string reducedTypeName = obj.GetType().Name;
            switch (reducedTypeName)
            {
                case "Class":
                    return "SeriesCollection";
                case "Series":
                    return "ExercisesCollection";
                case "Exercise":
                    return "TasksCollection";
                case "Semester":
                    return "ClassesCollection";
                default:
                    throw new Exception("couldnt form the collection name for object of type: " + obj.GetType());
                    break;
            }

           
            
        }

        /// <summary>
        /// Loads object graph from file, 
        /// passing non null user data will set progress value for tasks at creation time
        /// </summary>
        public static Semester LoadSTDoc(string pth, STUserData userData )
        {

            var d= XDocument.Load(pth);
            var ddoc = d.Root;
        
            Trace.Assert(ddoc.Name.LocalName == "STDoc");            
            return SemesterFromNode(ddoc.Element("SemesterDefinition"), userData?.BuildingBlocksData());
           
        }
        /// <summary>
        /// returns tru or moc data based on config
        /// </summary>
        /// <returns></returns>
        internal static Semester GetSemester()
        {
            Semester res;
            if (ConfigService.Instance.UseMockSemester || string.IsNullOrEmpty(ConfigService.Instance.CurrentSTDocPath))
            {
                Debug.WriteLine("Loading Mock Data");
                res  = DataServiceMock.LoadSTDoc(ConfigService.Instance.CurrentSTDocPath);
                res.Progress.Refresh();
                
            }
            else
            {
                Debug.WriteLine("Loading Real Data");
                string userDataPath = ConfigService.Instance.CurrentSTDocPath + ".stud.xml";
                bool shouldLoadUserData = File.Exists(userDataPath);
                STUserData user_data = null;
                if (shouldLoadUserData)
                {
                    user_data = STUserData.Load(userDataPath);
                }
                res = DataService.LoadSTDoc(ConfigService.Instance.CurrentSTDocPath, user_data);
            }
            return res;
        }

        private static TaskS TaskSFromNode(XElement taskElement, Exercise parent, Dictionary<Guid, BuildingBlockData> BuildingBlocksCompletion )
        {
            TaskS res = new TaskS();
            res.Name = taskElement.Attribute("CustomName").Value;
            ///todo: temporary code to correct the data with missing guids, generating new guids for the tasks
            if (taskElement.Attribute("ID") == null)
            {
                taskElement.SetAttributeValue("ID", Guid.NewGuid());
            }
            else if (taskElement.Attribute("ID").Value == "00000000-0000-0000-0000-000000000000")
            {
                taskElement.SetAttributeValue("ID", Guid.NewGuid());
            }
            //end of temporary code
            res.Guid = Guid.Parse( taskElement.Attribute("ID").Value);
            if (BuildingBlocksCompletion != null)
            {
                BuildingBlockData buildingBlockData;
                BuildingBlocksCompletion.TryGetValue(res.Guid, out buildingBlockData);
                res.Progress.RequireCompletion(buildingBlockData.Value);
                res.CompletionDate = buildingBlockData.CompletionDate;
            }
            Trace.Assert(!string.IsNullOrEmpty(res.Name));
            Trace.WriteLine(res.Name);
            if (string.IsNullOrEmpty(res.Name)) throw new Exception("null");
            return res;//todo tasks should have index and name
        }

        private static Exercise ExerciseFromTaskNode(XElement exerciseXElement, Series parent, Dictionary<Guid, BuildingBlockData> BuildingBlocksCompletion )
        {
            
            Exercise res = new Exercise();
            var x_coll= exerciseXElement.Element("TasksCollection");
            Trace.Assert(x_coll != null);
            var col = x_coll.Elements("Task").Select((x_task) => TaskSFromNode(x_task, res, BuildingBlocksCompletion)).ToArray();
            res.Tasks= new ObservableCollection<STBuildingBlock>(col);
            res.Title = exerciseXElement.Attribute("CustomName").Value;
            (res.Progress as ComplexProgress).Refresh();
            return res;
        }
        private static Series SeriesFromNode(XElement SeriesXElement, Class parent, Dictionary<Guid, BuildingBlockData> BuildingBlocksCompletion )
        {
            Series res = new Series();
            var x_coll = SeriesXElement.Element("ExercisesCollection");
            Trace.Assert(x_coll != null);
            var col = x_coll.Elements("ExerciseDefinition").Select((x_exer) => ExerciseFromTaskNode(x_exer, res, BuildingBlocksCompletion)).ToArray();
            var res_Usi= new Common.USI();
            Common.USI.TryParse(SeriesXElement.Attribute("USI").Value, out res_Usi);
            res.Usi = res_Usi;
            res.Exercises = new ObservableCollection<STContainer>(col);
            (res.Progress as ComplexProgress).Refresh();
            return res;
        }
        private static Class ClassFromNode(XElement ClassXElement, Dictionary<Guid, BuildingBlockData> BuildingBlocksCompletion)
        {
            Class res = new Class();
            var x_coll = ClassXElement.Element("SeriesCollection");
            res.Name = ClassXElement.Attribute("Name").Value;
            Trace.Assert(!string.IsNullOrEmpty(res.Name));
            Trace.Assert(x_coll != null);
            var col = x_coll.Elements("SeriesDefinition").Select((x_Class) => SeriesFromNode(x_Class, res, BuildingBlocksCompletion)).ToArray();
            res.Series = new ObservableCollection<STContainer>(col);
           
            res.ExamDate = DateTime.Parse(ClassXElement.Attribute("DeadlineDate").Value);
            (res.Progress as ComplexProgress).Refresh();
            return res;
        }
        private static Semester SemesterFromNode(XElement SemesterXElement, Dictionary<Guid, BuildingBlockData> BuildingBlocksCompletion)
        {
            Semester res = new Semester();
            var x_coll = SemesterXElement.Element("ClassesCollection");
            Trace.Assert(x_coll != null);
            res.Name = SemesterXElement.Attribute("Name").Value;
            Trace.Assert(!string.IsNullOrEmpty(res.Name));
            var col = x_coll.Elements("ClassDefinition").Select((x_Class) => ClassFromNode(x_Class, BuildingBlocksCompletion)).ToArray();
            
            res.Classes = new ObservableCollection<STContainer>(col);
            (res.Progress as ComplexProgress) .Refresh() ;
            
            return res;
        }

       

    }


}
