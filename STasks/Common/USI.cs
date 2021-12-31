using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace STasks.Common
{
    /// <summary>
    /// Unique series identifier: basic form: [*.]<ClassName>.serie<SerieNumber>[.Year][.*]
    /// examples:
    /// electrc.serie1.2016
    /// tollerance: undefined year, leading or traling parts, 
    /// electric.serie4
    /// whatever.electric.serie4.2014.whatever-ze.pdf
    /// electric.serie2.2014.pdf
    /// </summary>
    public struct USI : IFormattable
    {


        /// <summary>
        /// the original passed string if the USI was constructed by a string
        /// </summary>
        public readonly string originalString;
        public string ClassName { get; set; }
        /// <summary>
        /// -1 : undefined year
        /// </summary>
        public int Year { get; set; }
        public int SeriesNumber { get; set; }
        public static bool TryParse(string usi_as_string, out USI output)
        {
            Match m = Regex.Match(usi_as_string, @"([^.]+)\.serie(\d)\.?(\d{4})?", RegexOptions.IgnoreCase);
            if (m.Success == false)
            {
                output = new USI("", 1111, 1);
                return false;
            }
            USI res = new USI();
            res.ClassName = m.Groups[1].Value;
            res.SeriesNumber = int.Parse(m.Groups[2].Value);
            res.Year = m.Groups[3].Success ? int.Parse(m.Groups[3].Value) : -1;
            output = res;
            return true;

        }
        public USI(string usi_as_string)
        {

            Match m = Regex.Match(usi_as_string, @"([^.]+)\.serie(\d)\.?(\d{4})?", RegexOptions.IgnoreCase);
            if (m.Success == false)
            {
                throw new InvalidUSIException();
            }
            ClassName = m.Groups[1].Value;
            SeriesNumber = int.Parse(m.Groups[2].Value);
            Year = m.Groups[3].Success ? int.Parse(m.Groups[3].Value) : -1;
            originalString = usi_as_string;
        }
        public USI(string className, int year, int seresNumber)
        {
            ClassName = className; Year = year; SeriesNumber = seresNumber;
            originalString = null;
        }

        /// <summary>
        /// use orginalString property to retrieve the original passeed stirng if any
        /// </summary>
        public override string ToString()
        {

            return $"{ClassName}.serie{SeriesNumber}" + (Year == -1 ? "" : $".{Year}");
        }
       

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return format.Replace("IX", SeriesNumber.ToString()).Replace("Y", Year.ToString()).Replace("C", ClassName);
        }

        //"ELECTRONIQUE_ANALOGIQUE:serie2:2014"
    }
}
