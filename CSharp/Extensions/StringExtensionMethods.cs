using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Solution.Project.Extensions
{
    public static class StringExtensionMethods
    {
        /// <summary>
        /// Determines if stringToCheck starts with, and ends with specifiedValue.
        /// </summary>
        public static bool StartsAndEndsWith(this string stringToCheck, char specifiedValue)
        {
            if (stringToCheck == null)
            {
                return false;
            }

            if (stringToCheck.StartsWith(specifiedValue) && stringToCheck.EndsWith(specifiedValue))
            {
                return true;
            }

            return false;
        }


    }
}
