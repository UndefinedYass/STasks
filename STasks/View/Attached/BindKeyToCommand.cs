using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace STasks.View.Attached
{
    public class BindKeyToCommand
    {

        static TextBox tb;

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
            DependencyProperty.RegisterAttached("Key", typeof(Key), typeof(BindKeyToCommand), new PropertyMetadata( Key.None, onKeyChanged));





        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(BindKeyToCommand), new PropertyMetadata(null));






        public static bool GetSetKeyboardFocusOnMouseClick(DependencyObject obj)
        {
            return (bool)obj.GetValue(SetKeyboardFocusOnMouseClickProperty);
        }

        public static void SetSetKeyboardFocusOnMouseClick(DependencyObject obj, bool value)
        {
            obj.SetValue(SetKeyboardFocusOnMouseClickProperty, value);
        }

        // Using a DependencyProperty as the backing store for SetKeyboardFocusOnMouseClick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SetKeyboardFocusOnMouseClickProperty =
            DependencyProperty.RegisterAttached("SetKeyboardFocusOnMouseClick", typeof(bool), typeof(BindKeyToCommand), new PropertyMetadata(false, onSetKeyboardFocusOnMouseClickChanged));

        private static void onSetKeyboardFocusOnMouseClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = d as FrameworkElement;
            if (fe == null) return;
            if (ClassCardProperties.HasToogledOn(e))
            {
                fe.MouseDown += handleMouseDown;
            }
            else if (ClassCardProperties.HasToogledOn(e))
            {
                fe.MouseDown -= handleMouseDown;
            }

        }

        private static void handleMouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe == null) return;
            Keyboard.Focus(fe);
        }

        private static void onKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = d as FrameworkElement;
            if (fe == null) return;
            if (e.NewValue is Key && (((Key)e.NewValue) != Key.None))
            {
                fe.KeyDown += handleKeyDown;
            }
            else 
            {
                fe.KeyDown -= handleKeyDown;
            }
        }

        private static void handleKeyDown(object sender, KeyEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe == null) return;
            if (e.Key == GetKey(fe))
            {
                ICommand com = GetCommand(fe);
                if (com != null) com.Execute(null);
            }
            
        }
    }
}
