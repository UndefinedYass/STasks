using STasks.Model.Explorer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace STasks.Common.Converters
{
    [ValueConversion(typeof(ExplorerElementsAttributes), typeof(ImageSource))]
    public class FileAttributesToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            ExplorerElementsAttributes attr = value as ExplorerElementsAttributes;
           
            if (attr.IsIncluded == false)
            {
                return new BitmapImage(new Uri($"pack://application:,,,/Media/Icons/explorer-element-file-exluded.png"));
            }
            else
            {
                string CustomFileIconPath = "";
                if (TryGetCustomFileIcons(attr.FileExtension, out CustomFileIconPath))
                {
                    return new BitmapImage(new Uri(CustomFileIconPath));
                }
                else
                {
                    return new BitmapImage(new Uri($"pack://application:,,,/Media/Icons/explorer-element-file.png"));
                }
            }
           
           

        }
        /// <summary>
        /// lookes in the resources directory, looking for an icon wthat targets the extension in question
        /// these icons files are exposed to the user to add/remove/replace under the path ./Resources/Icons/
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <param name="customFileIconPath"></param>
        /// <returns></returns>
        private bool TryGetCustomFileIcons(string fileExtension, out string customFileIconPath)
        {
            string TargetIconPath = Path.Combine(MI.MAIN_PATH, $"Resources/Icons/ExplorerElement-{fileExtension}.png");//todo document specifications
            if (File.Exists(TargetIconPath))
            {
                customFileIconPath = TargetIconPath;
                return true;
            }
            else
            {
                customFileIconPath = string.Empty;
                return false;
            }
        }

       

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
