using STasks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace STasks.View.TemplateSelectors
{
    public class MMTabContentTemplateSelector: DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement elem = container as FrameworkElement;
            DataTemplate HomeTemplate = ((DataTemplate)elem.FindResource("HomeTabContentTemplate"));
            DataTemplate ClassTemplate = ((DataTemplate)elem.FindResource("ClassTabContentTemplate"));
            if (item != null && item is ITabContent)
            {
                if (((ITabContent)item).IsSpecialHomeTab) return HomeTemplate;
                else return ClassTemplate;
            }
            else return null;

        }
    }



    public class ExplorerTreeItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement elem = container as FrameworkElement;
            DataTemplate FolderTemplate = ((DataTemplate)elem.FindResource("TreeHeaderTemplate_Folder"));
            DataTemplate FileTemplate = ((DataTemplate)elem.FindResource("TreeHeaderTemplate_File"));
            if (item != null && item is Model.Explorer.IExplorerElement)//nt type when implemented
            {
                switch (((Model.Explorer.IExplorerElement)item).Type)
                {
                    case Model.Explorer.ExplorerElementType.Folder:
                        return FolderTemplate;
                    case Model.Explorer.ExplorerElementType.File:
                        return FileTemplate;
                    default:
                        return null;
                }
            }
            else return null;

        }
    }

}
