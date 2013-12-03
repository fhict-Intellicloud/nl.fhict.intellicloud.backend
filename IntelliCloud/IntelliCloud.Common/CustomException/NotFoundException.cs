namespace nl.fhict.IntelliCloud.Common.CustomException
{
    /// <summary>
    /// Generic exception class that can be used when a requested item cannot be found.
    /// </summary>
    public class NotFoundException : System.Exception
    {
        /// <summary>
        /// Default constructor; needs to be defined explicitly since it would be gone otherwise.
        /// </summary>
        public NotFoundException()
        {
        }

        /// <summary>
        /// Default constructor that allows a message to be passed to describe the exception.
        /// </summary>
        /// <param name="message">The description of the exception.</param>
        public NotFoundException(string message) : base(message)
        {
        }
    }
}