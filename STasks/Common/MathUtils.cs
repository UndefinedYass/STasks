using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STasks.Common
{
    public static class MathUtils
    {
        static Random r = new Random();
        //1,2
        public static int RandInt(int includedMin, int  includedMax)
        {
            
            int length = includedMax - includedMin + 1;
            Double rf = r.NextDouble() * length;
            return  Math.Min( (int)Math.Floor(rf+ includedMin),includedMax);//the min nsures that includedMax is not ecceded in rare case when rf=1.0
        }
        
        public static bool RndBool()
        {
            bool res= r.NextDouble() > 0.5;
           return res;
        }
        public static bool RndBasedBool(double bias)
        {

            return RndBiasedSource(bias) > 0.5;
        }

        public static double RndBiasedSource(double bias)
        {
            double rf = r.NextDouble();
            double bias_w =1- Math.Pow( r.NextDouble(),3);
            double rf_w = 1 - bias_w;
            return ((bias*bias_w) + (rf*rf_w))  /( bias_w+rf_w);
        }

        /// <summary>
        /// (1/(rac(2pi)*a))exp(-(x^2)/(2*((a)^2)))
        /// NOTE: the approximat curv with is Pi*a (by mi)
        /// the curv peak is 1/ sqrt(2pi)*a approximately: 0.398/a
        /// the curv hight is 
        /// </summary>
        /// <param name="x">variable</param>
        /// <param name="a">higher a values give a bell-curv with higher peak and lower width</param>
        /// <param name="b">directlt tweaks the horizontal offset of the curv</param>
        /// <returns></returns>
        public static double GausianKernel(double x, double a=1, double b=0)
        {
            return 1 / (a * Math.Sqrt(2 * Math.PI)) *
                (Math.Exp(-(Math.Pow((x-b),2)) / (2 * (Math.Pow(a, 2)))));
        }

    }
}
