using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTwitterClient.Business
{
    public class Validation
    {
        /// <summary>
        /// Checks if the string is not null or empty.
        /// </summary>
        /// <param name="value">The string that needs to be checked</param>
        public static void StringCheck(string value)
        {
            if (value == null)
                throw new ArgumentNullException();
            else if (String.IsNullOrEmpty(value))
                throw new ArgumentException("String is empty.");
        }

        public static void TweetCheck(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Tweet is empty");
            else if (value.Length > 140)
                throw new ArgumentException("Tweet can't be longer then 140 characters");
        }
    }
}
