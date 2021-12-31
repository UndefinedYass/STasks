using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Converters
{




    public class DateTimeUtils
    {
        public static string FormatDate(DateTime d)
        {
            string[] months = "Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec".Split(',');
             return $"{d.Day} {months[d.Month - 1]}"; 
           
        }
    }

    [ValueConversion(typeof(DateTime?), typeof(TimeSpan?))]
    class NullableDateTimeToTimeSpan : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            else if (value.GetType() == typeof(DateTime))
            {
                DateTime dt = (DateTime)value;
                if (targetType == typeof(DateTime))
                {
                    return dt ;

                }
                else if(targetType == typeof(TimeSpan))
                {
                    return dt.TimeOfDay;
                }
                else
                {
                    throw new Exception($"time conversion target type  {targetType} not supported");
                }
            }
            else if (value.GetType() == typeof(TimeSpan))
            {
                TimeSpan ts = (TimeSpan)value;
                if (targetType == typeof(DateTime?))
                {
                    return new DateTime(ts.Ticks);

                }
                else if (targetType == typeof(TimeSpan))
                {
                    return ts;
                }
                else
                {
                    throw new Exception($"time conversion target type {targetType} not supported");
                }
            }
            else
            {
                throw new Exception($"time conversion cannot parse type {value.GetType()}   ");
            }

            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            else if (value.GetType() == typeof(DateTime))
            {
                DateTime dt = (DateTime)value;
                if (targetType == typeof(DateTime))
                {
                    return dt;

                }
                else if (targetType == typeof(TimeSpan))
                {
                    return dt.TimeOfDay;
                }
                else if (targetType == typeof(TimeSpan?))
                {
                    return dt.TimeOfDay;
                }
                else
                {
                    throw new Exception($"time conversion back target type  {targetType} not supported");
                }
            }
            else if (value.GetType() == typeof(TimeSpan))
            {
                TimeSpan ts = (TimeSpan)value;
                if (targetType == typeof(DateTime))
                {
                    return new DateTime(ts.Ticks);

                }
                else if (targetType == typeof(TimeSpan))
                {
                    return ts;
                }
                else
                {
                    throw new Exception($"time conversion back target type {targetType} not supported");
                }
            }
            else
            {
                throw new Exception($"time conversion back cannot parse type {value.GetType()}   ");
            }


        }
    }
}
