using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STasks.Services;
using STasks.Model;
using STasks.Common;
using System.Diagnostics;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //test: creat ex with two tasks, assign tasks's progress and query the ex's progress


            Semester s = new Semester();
            Class c1 = new Class();
            Series s1 = new Series();
            Exercise ex = new Exercise();
            TaskS t = new TaskS();

            ex.Children.Add(t);
            s1.Containers.Add(ex);
            c1.Containers.Add(s1);
            s.Containers.Add(c1);
            t.Progress.RequireCompletion(true);


            s.Name = "MyTestSemester";
            
            
            DataService.SaveSTDoc(s, @"C:\TOOLS\STasks\STDoc-XML Schema\saved.xml");
            bool a = ex.Progress.IsComplete == false;
            string a_str = a.ToString();
            string a_str2 = a_str + " okaay";
            string a_str3 = a_str.Split(' ')[0].ToLower();
            bool aback = bool.Parse(a_str3);
            Debug.WriteLine("aback: "+aback);
            Debug.WriteLine($"commp w: {s.Progress.Consolidated.CompletedWeight}");
            Debug.WriteLine($"total w: {s.Progress.Consolidated.TotalWeight}");

            string my_str = "false";
            bool my_aback = bool.Parse(my_str);
            Assert.IsFalse(my_aback);
            Assert.IsFalse(true);
            //Assert.IsFalse(aback);
            Assert.IsFalse(my_aback);
            Assert.IsFalse(my_aback);
            Assert.IsTrue(t.Progress.IsComplete == true, "requring true test3");
            Assert.IsTrue(t.Progress.IsComplete == true, "requring true test2");

            //Assert.IsTrue(t1.Progress.IsComplete == true && t2.Progress.IsComplete == true, "requring true test");

            //Assert.IsTrue(x.Progress.Consolidated.CompletedWeight == 3, $"nah: {x.Progress.Consolidated.CompletedWeight}");
        }

        [TestMethod]
        public void DataServiceSavingTest()
        {
            //Semester : SemesterDefinition
            //Class : ClassDefinition
            //Series : SeriesDefinition
            //Exercise : ExerciseDefinition
            //TaskS : Task
            TaskS t = new TaskS();
            Exercise e = new Exercise();
            Series s = new Series();
            Class c = new Class();
            Semester sm = new Semester();


            Assert.IsTrue(DataService.GetXElementTagName(t) == "Task");
            Assert.IsTrue(DataService.GetXElementTagName(e) == "ExerciseDefinition");
            Assert.IsTrue(DataService.GetXElementTagName(s) == "SeriesDefinition");
            Assert.IsTrue(DataService.GetXElementTagName(c) == "ClassDefinition");
            Assert.IsTrue(DataService.GetXElementTagName(sm) == "SemesterDefinition");

        }
    }
}
