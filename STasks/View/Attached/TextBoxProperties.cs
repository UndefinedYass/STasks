using STasks.ViewModel;
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
    public class TextBoxProperties
    {



        public static bool GetInvokeLostFocusOrEnterCallback(DependencyObject obj)
        {
            return (bool)obj.GetValue(InvokeLostFocusOrEnterCallbackProperty);
        }

        public static void SetInvokeLostFocusOrEnterCallback(DependencyObject obj, bool value)
        {
            obj.SetValue(InvokeLostFocusOrEnterCallbackProperty, value);
        }

        // Using a DependencyProperty as the backing store for InvokeLostFocusOrEnterCallback.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InvokeLostFocusOrEnterCallbackProperty =
            DependencyProperty.RegisterAttached("InvokeLostFocusOrEnterCallback", typeof(bool), typeof(TextBoxProperties), new PropertyMetadata(false, onInvokeLostFocusOrEnterCallbackChanged));





        public static bool GetIsAllSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAllSelectedProperty);
        }

        public static void SetIsAllSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAllSelectedProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsAllSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllSelectedProperty =
            DependencyProperty.RegisterAttached("IsAllSelected", typeof(bool), typeof(TextBoxProperties), new PropertyMetadata(false,OnIsAllSelectedChanged));

        private static void OnIsAllSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (ClassCardProperties.HasToogledOn(e))
            {
                TextBox tb = (d as TextBox);
                tb.SelectAll();
                tb.Dispatcher.BeginInvoke( new Action( () =>
                {
                    Keyboard.Focus(tb);
                    
                    
                }) );

               
            }
        }

        private static void onInvokeLostFocusOrEnterCallbackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (ClassCardProperties.HasToogledOn(e))
            {
                //register events handelers
                TextBox tb = (d as TextBox);
                tb.LostFocus += TbLostFousEventHandler;
                tb.KeyDown += TbKeyDownEventHandler;

            }
            else if (ClassCardProperties.HasToogledOff(e))
            {
                //unregister events handelers
                TextBox tb = (d as TextBox);
                tb.LostFocus -= TbLostFousEventHandler;
                tb.KeyDown -= TbKeyDownEventHandler;
            }
        }

        private static void TbKeyDownEventHandler(object sender, KeyEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            IRenamingTextBoxViewModel vm = (IRenamingTextBoxViewModel)tb.DataContext;
            if (e.Key == System.Windows.Input.Key.Enter) vm.TextBoxLostFocusOrEnterKeyCallBack(false);
        }

        private static void TbLostFousEventHandler(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            IRenamingTextBoxViewModel vm = (IRenamingTextBoxViewModel)tb.DataContext;
            vm.TextBoxLostFocusOrEnterKeyCallBack(true);

        }
    }
}
