using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nl.fhict.IntelliCloud.Common.CustomException
{
    /// <summary>
    /// Generic exception class that can be used when a logged in user doesn't satify specific requirements.
    /// </summary>
    public class AccesDeniedException : System.Exception
    {
        /// <summary>
        /// Default constructor; needs to be defined explicitly since it would be gone otherwise.
        /// </summary>
        public AccesDeniedException()
        {
        }

        /// <summary>
        /// Default constructor that allows a message to be passed to describe the exception.
        /// </summary>
        /// <param name="message">The description of the exception.</param>
        public AccesDeniedException(string message)
            : base(message)
        {
        }
    }
}
