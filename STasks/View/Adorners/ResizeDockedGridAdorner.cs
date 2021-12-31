using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace STasks.View.Adorners
{
    public enum ResizeDockedGridAdornerType { left,right,top}
    public class ResizeDockedGridAdorner : Adorner
    {
        public ResizeDockedGridAdorner(UIElement adornedElem, ResizeDockedGridAdornerType type ) :base(adornedElem)
        {
            visualCollection = new VisualCollection(this);
            MainThumb = new Thumb();
            MainThumb.Template = (AdornedElement as FrameworkElement).FindResource("ResizeAdornerThumbTemplate") as ControlTemplate;
            Type = type;
            
            MainThumb.BorderThickness = new Thickness(0,0,0,0);
            MainThumb.BorderBrush = ( AdornedElement as FrameworkElement).FindResource("MaterialDesignDivider") as Brush;
            MainThumb.Background = new SolidColorBrush(Colors.Transparent);
            MainThumb.DragDelta += HandleMainThumbDragDelta;
            switch (Type)
            {
                case ResizeDockedGridAdornerType.left:
                    MainThumb.Cursor = Cursors.SizeWE;
                    MainThumb.Width = 4;
                    break;
                case ResizeDockedGridAdornerType.right:
                    MainThumb.Cursor = Cursors.SizeWE;
                    MainThumb.Width = 4;
                    break;
                case ResizeDockedGridAdornerType.top:
                    MainThumb.Cursor = Cursors.SizeNS;
                    MainThumb.Height = 4;
                    break;
                default:
                    break;
            }
          
            visualCollection.Add(MainThumb);
        }
        ResizeDockedGridAdornerType Type { get; set; }
        Thumb MainThumb { get; set; }
        private void HandleMainThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement fe = AdornedElement as FrameworkElement;
            switch (Type)
            {
                case ResizeDockedGridAdornerType.left:
                    fe.Width = Math.Max(100, AdornedElement.DesiredSize.Width - e.HorizontalChange);
                    break;
                case ResizeDockedGridAdornerType.right:
                    fe.Width = Math.Max(100, AdornedElement.DesiredSize.Width + e.HorizontalChange);
                    break;
                case ResizeDockedGridAdornerType.top:
                    fe.Height = Math.Max(100, AdornedElement.DesiredSize.Height - e.VerticalChange);
                    break;
                default:
                    break;
            }
            //# case 
           
        }

        VisualCollection visualCollection { get; set; }


        protected override int VisualChildrenCount
        {
            get
            {
                return visualCollection.Count;
            }
        }
        protected override Visual GetVisualChild(int index)
        {
            return visualCollection[index];
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            FrameworkElement fe = AdornedElement as FrameworkElement;
            switch (Type)
            {
                case ResizeDockedGridAdornerType.left:
                    MainThumb.Arrange(new Rect(0, 0, 4, fe.ActualHeight));
                    break;
                case ResizeDockedGridAdornerType.right:
                    MainThumb.Arrange(new Rect(fe.ActualWidth, 0, 4, fe.ActualHeight));
                    break;
                case ResizeDockedGridAdornerType.top:
                    MainThumb.Arrange(new Rect(0, 0, fe.ActualWidth ,4 ));
                    break;
                default:
                    break;
            }
           
            return finalSize;

        }
       

    }
}
