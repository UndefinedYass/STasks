using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STasks.Common
{
    public class InvalidUSIException : Exception
    {
        public InvalidUSIException() : base("Invalid UniqueSeriesIdentifier string cannot be parsed")
        {

        }
    }
    class Exceptions
    {
    }
}
