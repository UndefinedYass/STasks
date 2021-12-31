using STasks.Model;
using STasks.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace STasks.View.Attached
{
    public delegate void DevMacroFunc();
    public class Macro
    {
        public Macro(string name, DevMacroFunc action, Key  key_ =Key.None )
        {
            Name = name; Action = action;
            key = key_;
        }
        public Key key { get; set; } 
        public string Name { get; set; }
        public DevMacroFunc Action {get;set;}
    }
    public class DevMacros
    {
        private static DevMacros _instance;
        public static DevMacros Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DevMacros();
                return _instance;

            }
        }

        public DevMacros()
        {
            Macros = new List<Macro>();
            Macros.Add(new Macro("Add random completion date information", () => {
                //todo remove this obsolete macro
                var s =Model.STUserData.Load(@"C:\TOOLS\STasks\STDoc-XML Schema\saved2.xml.stud.xml");
                for (int i = 0; i < s.BuildingBlocksCompletionArray.Count; i++)
                {
                    var ne = s.BuildingBlocksCompletionArray[i];
                    ne.CompletionDate = DateTime.Now - TimeSpan.FromDays(Common.MathUtils.RandInt(0, 10));
                    s.BuildingBlocksCompletionArray[i] = ne;
                }
                 Trace.WriteLine("loaded user data and assigned dates");

                s.Save(@"C:\TOOLS\STasks\STDoc-XML Schema\saved2.xml.stud.xml", true);
            }, Key.R));

            Macros.Add(new Macro("Pushing plotable data", () =>
            {
                //todo remove this obsolete macro

                Trace.WriteLine("loaded user data and assigned dates");

                var mvm = ((App.Current.MainWindow as MainWindow).DataContext as ViewModel.MainViewModel);

                Services.PlotableData pd = new Services.PlotableData();
                ObservableCollection<Services.PlotingService.ICompletionPiece> lst = new ObservableCollection<Services.PlotingService.ICompletionPiece>();
                var ud = Model.STUserData.Load(@"C:\TOOLS\STasks\STDoc-XML Schema\saved2.xml.stud.xml");
                foreach (TaskS item in mvm.LoadedSemester.GetAllChildAndDescendantBuildingBlocks())
                {

                    lst.Add(new Services.CompletionPiece() { CompletionDate =item.CompletionDate, CompletedBuildingBlockWeight = item.Progress.Weight });
                    Trace.WriteLine(item.CompletionDate);
                }
                
                pd.CompletionPiece = new System.Collections.ObjectModel.ReadOnlyObservableCollection<Services.PlotingService.ICompletionPiece>(lst);
                pd.GoalDeadLine = DateTime.Now + TimeSpan.FromDays(30);
                pd.GoalWeight = 69;
                pd.TimeStart = DateTime.Now - TimeSpan.FromDays(30);
                
                Services.PlotingService.Instance.PushData(pd);



            }, Key.None));
        }

        public List<Macro> Macros { get; set; }

















        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }


        public static bool HasToogledOn(DependencyPropertyChangedEventArgs e)
        {
            return (((e.OldValue as bool?) == null || (e.OldValue as bool?) == false)
                && ((e.NewValue as bool?) == true));
        }
        public static bool HasToogledOff(DependencyPropertyChangedEventArgs e)
        {
            return (((e.NewValue as bool?) == null || (e.NewValue as bool?) == false)
                && ((e.OldValue as bool?) == true));
        }
        // Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(DevMacros), new PropertyMetadata(false, onIsEnabledChanged));

        private static void onIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = d as FrameworkElement;
            if (!(fe is Window)) return;
            Window w = (Window)fe;
            if (HasToogledOn(e))
            {
                w.KeyDown += handleKeyDown;
            }
            else if (HasToogledOff(e))
            {
                w.KeyDown -= handleKeyDown;
            }
           

        }

        private static void handleKeyDown(object sender, KeyEventArgs e)
        {
            var target_macro = Instance.Macros.FirstOrDefault((m) => m.key == e.Key);
            if (target_macro != null)
            {
                Trace.WriteLine($"Executing dev macro '{target_macro.Name}'..");
                target_macro?.Action();
            }
            
        }
    }
}
