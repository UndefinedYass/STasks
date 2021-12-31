using STasks.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace STasks.View.Attached
{
    public class DevHitBreakPoint
    {





        public static Key GetKey(DependencyObject obj)
        {
            return (Key)obj.GetValue(KeyProperty);
        }

        public static void SetKey(DependencyObject obj, Key value)
        {
            obj.SetValue(KeyProperty, value);
        }

        // Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.RegisterAttached("Key", typeof(Key), typeof(DevHitBreakPoint), new PropertyMetadata(Key.None, onKeyChanged));

        private static void onKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = d as FrameworkElement;
            if (fe == null) return;
            if (e.NewValue is Key && (((Key)e.NewValue) != Key.None))
            {
                fe.KeyDown += handleKeyDown;
                fe.MouseDown += handleMouseDown;

            }
            else
            {
                fe.KeyDown -= handleKeyDown;
                fe.MouseDown -= handleMouseDown;
            }
        }

        private static void handleMouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe == null) return;
            Trace.WriteLine("dev break point element is focused");
            fe.Dispatcher.BeginInvoke(new Action(() => {
                fe.Focusable = true;
                fe.Focus();
                Keyboard.Focus(fe);
            }));
            
        }

        private static void handleKeyDown(object sender, KeyEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe == null) return;
            if (e.Key == GetKey(fe))
            {
                HandleHitBPCommand(fe);
                
            }
        }

    
        private static void HandleHitBPCommand(FrameworkElement fe)
        {

            Debug.WriteLine("bp hit command");
            
        }

     





    }
}
