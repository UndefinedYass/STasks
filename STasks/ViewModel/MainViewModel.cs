using STasks.Model;
using STasks.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;


namespace STasks.ViewModel
{
    public class MainViewModel: BaseViewModel
    {

        

        [DllImport("user32.dll")]
        static extern int MessageBox(IntPtr hWnd, string text, string caption,int type);
        public static void Maini()
        {
           
            
            MessageBox(IntPtr.Zero,
            "Please do not press this again.", "Attention", 0);
        }
        public MainViewModel()
        {


            // CurrentClassViewModel = new ClassViewModel(Common.Mock.GetDummyClass("Electronics"));

            if(string.IsNullOrEmpty(ConfigService.Instance.CurrentSTDocPath))
            {
                LoadedSemester = DataServiceMock.LoadSTDoc(ConfigService.Instance.CurrentSTDocPath);
            }
            else
            {
                LoadedSemester = DataService.LoadSTDoc(ConfigService.Instance.CurrentSTDocPath);
            }
            
            CurrentSemesterViewModel = new SemesterViewModel(LoadedSemester);
            Tabs = new ObservableCollection<ITabContent>(new ITabContent[] {
                CurrentSemesterViewModel,
                CurrentSemesterViewModel.ClassesVMS[1],
            });
            CurrentSelectedTab = CurrentSemesterViewModel;
        }

        private static MainViewModel _instance;
        public static MainViewModel Instance{get{if(_instance == null){_instance = new MainViewModel();}return _instance;}}

        public Semester LoadedSemester { get; set; }











        




        private SemesterViewModel _CurrentSemesterViewModel;
        public SemesterViewModel CurrentSemesterViewModel
        {
            set { _CurrentSemesterViewModel = value; notif(nameof(CurrentSemesterViewModel)); }
            get { return _CurrentSemesterViewModel; }
        }


        public ObservableCollection<ITabContent> Tabs
        {
            get; internal set;
        }




        private ITabContent _CurrentSelectedTab;
        public ITabContent CurrentSelectedTab
        {
            set { _CurrentSelectedTab = value; notif(nameof(CurrentSelectedTab)); }
            get { return _CurrentSelectedTab; }
        }


        public void OpenTabAndSelect(Class c)
        {


            var temp = OpenTab(c);
            if (temp != null) CurrentSelectedTab = temp;
        }
        public ITabContent OpenTab(Class c)
        {
            var target_class_vm = CurrentSemesterViewModel.ClassesVMS.FirstOrDefault((cvm) => cvm.ModelClass == c);
            if (target_class_vm == null)
            {
                Trace.WriteLine("something went wrong finding class view model");
                return null;
            }
            else
            {
                if (Tabs.Contains(target_class_vm)){
                    //already opened
                }
                else
                {
                    Tabs.Add(target_class_vm);
                }
                return target_class_vm;
            }
        }

        private void CloseTab(Class c)
        {
            var target_class_vm = CurrentSemesterViewModel.ClassesVMS.FirstOrDefault((cvm) => cvm.ModelClass == c);
            if (target_class_vm == null)
            {
                Trace.WriteLine("something went wrong finding class view model in closing tab");
            }
            else
            {
                Tabs.Remove(target_class_vm);
            }
        }




        /* private ITabContent _CurrentSelectedTab;
         public ITabContent CurrentSelectedTab
         {
             set { _CurrentSelectedTab = value;
                 notif(nameof(CurrentSelectedTab));

             }
             get { return _CurrentSelectedTab; }
         }*/









        public ICommand LoadSemesterCommand
        {
            get { return new Common.MICommand(handleLoadSemesterCommand); }
        }
       

      

        private void handleLoadSemesterCommand()
        {
            string test_stdoc = @"C:\TOOLS\STasks\STDoc-XML Schema\s5stdocv1.xml";
            Semester s =  Services.DataService.LoadSTDoc(test_stdoc);
            LoadedSemester = s;
            CurrentSemesterViewModel.update(LoadedSemester);
            
            
            notif(nameof(CurrentSemesterViewModel));
            //removing opened class tabs if an
            var toremove = new List<ITabContent>();
            foreach (var item in Tabs)
            {
                if (item.IsSpecialHomeTab == false)
                    toremove.Add(item);
            }
            foreach (var item in toremove)
            {
                Tabs.Remove(item);
            }
            toremove = null;
            /*Tabs = new ObservableCollection<IMMTabHeader>(new IMMTabHeader[] {
                new MMHomeTabHeader() { },
               
            });*/
        }

        
    }

    
    public interface ITabContent
    {
        bool IsSpecialHomeTab { get;  }
        bool IsDirty { get; }
        string Title { get; }
    }
   
    public class pd  
    {
        
    }
    
}
