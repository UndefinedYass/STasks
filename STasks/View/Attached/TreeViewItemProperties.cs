using STasks.Model.Explorer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace STasks.View.Attached
{
    public class TreeViewItemProperties
    {


        public static bool GetFireCommandEventOnExpanded(DependencyObject obj)
        {
            return (bool)obj.GetValue(FireCommandEventOnExpandedProperty);
        }

        public static void SetFireCommandEventOnExpanded(DependencyObject obj, bool value)
        {
            obj.SetValue(FireCommandEventOnExpandedProperty, value);
        }

        // Using a DependencyProperty as the backing store for FireCommandEventOnExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FireCommandEventOnExpandedProperty =
            DependencyProperty.RegisterAttached("FireCommandEventOnExpanded", typeof(bool), typeof(TreeViewItemProperties), new PropertyMetadata(false, onFireCommandEventOnExpandedChanged));

        private static void onFireCommandEventOnExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null || !(d is TreeViewItem)) return;
            TreeViewItem TI = d as TreeViewItem;
            if (ClassCardProperties.HasToogledOn(e))
            {
                TI.Expanded += HandleExpanded;
            }
            else if (ClassCardProperties.HasToogledOff(e))
            {
                TI.Expanded -= HandleExpanded;
            }
        }

        private static void HandleExpanded(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("Expanded");
            TreeViewItem TI = sender as TreeViewItem;
            if( TI.DataContext is Model.Explorer.IExpandedCommandEvent)
            {
                Trace.WriteLine("Expanded: executing");
                ((IExpandedCommandEvent)TI.DataContext).ExpandedCommandEvent.Execute(null);
            }
        }
    }
}
