using STasks.Model.Explorer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace STasks.Common.Converters
{
    [ValueConversion(typeof(ExplorerElementsAttributes), typeof(ImageSource))]
    public class FolderAttributesToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ExplorerElementsAttributes attr = value as ExplorerElementsAttributes;
            string icon_path;
            if (attr.IsIncluded)
            {
                if (attr.IsClass)
                {

                    icon_path = "Media/Icons/explorer-element-folder-class.png";
                   
                }
                else
                {
                    icon_path = "Media/Icons/explorer-element-folder.png";
                }

            }
            else
            {
                icon_path = "Media/Icons/explorer-element-folder-exluded.png";
            }
            return new BitmapImage(new Uri($"pack://application:,,,/{icon_path}"));

     
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
