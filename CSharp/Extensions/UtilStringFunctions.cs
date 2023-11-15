using System;
using System.Text;
using Argo.ECD.Common.Extensions;
using Argo.Libraries.Logging.Contracts;

namespace Company.Solution.Project.Util
{
	// Useful string functions
    public static class UtilStringFunctions
    {

        public static string PadRight(string data, int length, char character = ' ')
        {
            data = data ?? string.Empty;
            return (data.Length <= length)   // Less than or equal to
                ? data.PadRight(length, character)
                : data.Substring(0, length);
        }

        public static string PadLeft(string data, int length, char character = ' ')
        {
            data = data ?? string.Empty;
            return (data.Length <= length)   // Less than or equal to
                ? data.PadLeft(length, character)
                : data.Substring(0, length);
        }

        public static string TakeRightmostCharacters(string data, int maxLength)
        {
            if (string.IsNullOrEmpty(data) || data.Length <= maxLength)
                return data;

            int startIndex = data.Length - maxLength;
            return data.Substring(startIndex);
        }

        public static string FormatUniqueId(string uniqueId)
        {
            var uniqueIdRawNoDashes = uniqueId.Replace("-", "");
            uniqueIdRawNoDashes = PadRight(uniqueIdRawNoDashes, 32);
            return uniqueIdRawNoDashes;
        }

        public static string FormatPhoneNumber(string number)
        {
            string phoneFormat = "###-###-####";
            return Convert.ToInt64(number).ToString(phoneFormat);
        }

        public static string ReplaceControlCharacters(string inString, char replaceChar)
        {
            if (inString == null) return null;
            StringBuilder newString = new StringBuilder();
            foreach (var ch in inString)
            {
                newString.Append(char.IsControl(ch) ? replaceChar : ch);
            }
            return newString.ToString();
        }
	}
}
