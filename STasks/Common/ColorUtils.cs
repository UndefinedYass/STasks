using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
namespace STasks.Common
{
    public class ColorUtils
    {
        private static int mixInt(int x1, int x2, float x1Ratio)
        {
            return (int) (((float) (x2 - x1)) * x1Ratio + x1);
        }
        /// <summary>
        /// 1 : green 3 193 14
        /// 0 : red  236 15 15
        /// </summary>
        public static Color GreenRedScale(float value)
        {
            int r = mixInt(3,236,value);
            int g = mixInt(193,15,  value);
            int b = 15;
            return Color.FromArgb(255, Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
        }
        /// <summary>
        /// 1 : green 106h 32l 89s
        /// 0 : red  0h  32l 89s
        /// </summary>
        public static Color GreenRedScaleHLS(float value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// optimized for white background
        /// </summary>
        /// <param name="value">normalized float, 0 gives pure red, 1 gives green</param>
        public static Color GreenRedScaleDiscret(float value)
        {
            // do as much colors as u want, start from pure red and end at green
            string[] colors = new[] { "#FFF10C0C", "#FFE8710D", "#FFE29F0F", "#FF7EB606", "#FF03C10F" };
            for (int i = 0; i < colors.Length; i++)
            {
                
                float stretched_val = value * (colors.Length);
                if(stretched_val>=i && stretched_val < i + 1)
                {
                    return (Color) ColorConverter.ConvertFromString( colors[i]) ;
                }
                if (value == 1)
                {
                    return (Color)ColorConverter.ConvertFromString(colors[colors.Length-1]);
                }
            }
            return Colors.Gray;
        }
    }
}
