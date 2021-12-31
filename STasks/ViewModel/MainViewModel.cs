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
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using STasks.Common;

namespace STasks.ViewModel
{
    public class MainViewModel: BaseViewModel
    {

        

       
        public MainViewModel()
        {


            // CurrentClassViewModel = new ClassViewModel(Common.Mock.GetDummyClass("Electronics"));

            LoadedSemester = DataService.GetSemester();
            
            CurrentSemesterViewModel = new SemesterViewModel(LoadedSemester);
            Tabs = new ObservableCollection<ITabContent>(new ITabContent[] {
                CurrentSemesterViewModel,
                CurrentSemesterViewModel.ClassesVMS[1],
            });

            LoadedSemester.Classes.CollectionChanged += HandleLodedSemesterClassesCollectionChanged;

            CurrentSelectedTab = CurrentSemesterViewModel;
        }

        private void HandleLodedSemesterClassesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action== NotifyCollectionChangedAction.Remove)
            {
                foreach (Class item in e.OldItems)
                {
                    ITabContent target_tab = Tabs.FirstOrDefault((t) => t is ClassViewModel && (t as ClassViewModel).ModelClass == item);
                    if (target_tab != null)
                    {
                        Tabs.Remove(target_tab);
                    }
                }
            }
        }



        private static MainViewModel _instance;
        public static MainViewModel Instance{get{if(_instance == null){_instance = new MainViewModel();}return _instance;}}

        public Semester LoadedSemester { get; set; }
        public Model.Explorer.ExplorerFolder Workspace { get; set; } = ExplorerService.Instance.WorkSpace;



        private bool _IsExplorerPanelVisible=true;
        public bool IsExplorerPanelVisible
        {
            set { _IsExplorerPanelVisible = value; notif(nameof(IsExplorerPanelVisible)); }
            get { return _IsExplorerPanelVisible; }
        }


        private bool _IsStatsPanelVisible=true;
        public bool IsStatsPanelVisible
        {
            set { _IsStatsPanelVisible = value; notif(nameof(IsStatsPanelVisible)); }
            get { return _IsStatsPanelVisible; }
        }
        /// <summary>
        /// pramaeter is interpreted to decide which panel to show or hide
        /// the string shoud have this format: panelName:True this sets the panel visible
        /// </summary>
        public ICommand ShowHidePanelCommand { get { return new MICommand<string>(HandleShowHidePanelCommand); } }

        public enum MainViewPanels { StatsPanel,ExplorerPanel}
        private void HandleShowHidePanelCommand(string param)
        {
            var m =Regex.Match(param, "(.*):(true|false)", RegexOptions.IgnoreCase);
            if (m.Success == false) throw new Exception("wrong parameter format for ShowHidePanelCommand at mainViewModel");
            bool val = bool.Parse(m.Groups[2].Value);
            MainViewPanels targetPanel = (MainViewPanels) Enum.Parse(typeof(MainViewPanels), m.Groups[1].Value) ;
            switch (targetPanel)
            {
                case MainViewPanels.StatsPanel:
                    IsStatsPanelVisible = val;
                    break;
                case MainViewPanels.ExplorerPanel:
                    IsExplorerPanelVisible = val;
                    break;
                default:
                    break;
            }
        }

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






        //SaveSemesterCommand


        public ICommand LoadSemesterCommand
        {
            get { return new Common.MICommand(handleLoadSemesterCommand); }
        }


        public ICommand SaveSemesterCommand
        {
            get { return new Common.MICommand(handleSaveSemesterCommand); }
        }

        public ICommand SaveSemesterAsCommand
        {
            get { return new Common.MICommand(handleSaveSemesterAsCommand); }
        }

        private void handleSaveSemesterAsCommand()
        {
            Common.IOUtils.PromptSavingPathAsync("xml", (pth, canceled) =>
            {
                if (canceled) return;
                DataService.SaveSTDoc(LoadedSemester,pth);
                DataService.SaveSTUserData(LoadedSemester, $"{pth}.stud.xml");
                MessageBox.Show($"saved files:{Environment.NewLine}{pth}{Environment.NewLine}{pth}.stud.xml");

            });
        }

        private void handleSaveSemesterCommand()
        {
            /*Semester s = new Semester();
            Class c1 = new Class();
            Series s1 = new Series();
            Exercise ex = new Exercise();
            TaskS t = new TaskS();

            ex.Children.Add(t);
            s1.Containers.Add(ex);
            c1.Containers.Add(s1);
            s.Containers.Add(c1);
            t.Progress.RequireCompletion(true);
            t.Name = "MyTask";
            c1.Name = "My Clss 1";

            s.Name = "MyTestSemester";

            */
            DataService.SaveSTDoc(LoadedSemester, @"C:\TOOLS\STasks\STDoc-XML Schema\saved2.xml");
            DataService.SaveSTUserData(LoadedSemester, @"C:\TOOLS\STasks\STDoc-XML Schema\saved2.xml.stud.xml");
           

        }



        private void handleLoadSemesterCommand()
        {


           
            string test_stdoc = @"C:\TOOLS\STasks\STDoc-XML Schema\s5stdocv1.xml";
            //Semester s =  Services.DataService.LoadSTDoc(test_stdoc);
            LoadedSemester = DataService. GetSemester();
            //todo removed an update call that may be important
            CurrentSemesterViewModel = new SemesterViewModel(LoadedSemester);
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
