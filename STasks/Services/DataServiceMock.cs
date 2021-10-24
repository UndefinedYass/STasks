using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STasks.Model;

namespace STasks.Services
{
    public class DataServiceMock 
    {
        /// <summary>
        /// uses the common.mock.cs to generate dummy semester content
        /// </summary>
        /// <param name="pth">no-op</param>
        /// <returns></returns>
        public static Semester LoadSTDoc(string pth)
        {
            return Common.Mock.GetDummySemester("Semester 5,3 2021-2022 (m)");
        }
    }
}
