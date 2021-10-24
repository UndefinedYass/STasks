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
            Exercise2 x = new Exercise2();
            Trace.WriteLine("ok");
            Task2 t1 = new Task2()
            {
                Parent = x,
                

            };
            Task2 t2 = new Task2()
            {
                Parent = x,
               

            };
            x.Containers = new System.Collections.ObjectModel.ObservableCollection<STContainer>();
            x.Children = new System.Collections.ObjectModel.ObservableCollection<STBuildingBlock>();
            x.Children.Add(t1);
            x.Children.Add(t2);
            x.Progress.Refresh();
            int totalCount = x.Progress.Consolidated.TotalCount;
            Console.WriteLine(totalCount);
            Assert.IsTrue(totalCount == 2, "totalCount test");
            Assert.IsTrue(x.Progress.Consolidated.CompletedCount == 0, "completed count test1");
            t2.Progress.ApplyValue(true);
            //x.Progress.Refresh();
            Assert.IsTrue(x.Progress.Consolidated.CompletedCount == 1, "completed count test2");


            t1.Progress.ApplyWeight(2) ;
            x.Progress.RequireCompletion(false);
            Assert.IsTrue(t1.Progress.IsComplete==false && t2.Progress.IsComplete==false, "requring false test");
            x.Progress.RequireCompletion(true);
            Assert.IsTrue(t1.Progress.IsComplete == true && t2.Progress.IsComplete == true, "requring true test");
            
            Assert.IsTrue(x.Progress.Consolidated.CompletedWeight == 3, $"nah: {x.Progress.Consolidated.CompletedWeight}");
        }
    }
}
