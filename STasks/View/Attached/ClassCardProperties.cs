using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace STasks.View.Attached
{
    public class ClassCardProperties
    {


        public static bool GetExecuteOpenActionOnDoubleClick(DependencyObject obj)
        {
            return (bool)obj.GetValue(ExecuteOpenActionOnDoubleClickProperty);
        }

        public static void SetExecuteOpenActionOnDoubleClick(DependencyObject obj, bool value)
        {
            obj.SetValue(ExecuteOpenActionOnDoubleClickProperty, value);
        }

        // Using a DependencyProperty as the backing store for ExecuteOpenActionOnDoubleClick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExecuteOpenActionOnDoubleClickProperty =
            DependencyProperty.RegisterAttached("ExecuteOpenActionOnDoubleClick", typeof(bool), typeof(ClassCardProperties), new PropertyMetadata(false, OnExecuteOpenActionOnDoubleClickChanged));



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

        private static void OnExecuteOpenActionOnDoubleClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control f = d as Control;
            if (HasToogledOn(e))
            {
                f.PreviewMouseDoubleClick += handleDoubleClick;
            }
            else if (HasToogledOff(e))
            {
                f.PreviewMouseDoubleClick -= handleDoubleClick;
            }
        }







        public static ICommand GetOpenCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(OpenCommandProperty);
        }

        public static void SetOpenCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(OpenCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for OpenCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenCommandProperty =
            DependencyProperty.RegisterAttached("OpenCommand", typeof(ICommand), typeof(ClassCardProperties), new PropertyMetadata(null));




        private static void handleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("handeling souble clck");
            var comm = GetOpenCommand(sender as Control);
            if(comm!=null && comm.CanExecute(null))
            {
                comm.Execute(null);
            }
        }
    }
}
