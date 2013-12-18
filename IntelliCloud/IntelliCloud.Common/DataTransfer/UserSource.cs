namespace nl.fhict.IntelliCloud.Common.DataTransfer
{
    /// <summary>
    /// A class containing a source for a user. A source is a external system that the user uses, like mail, Twitter or
    /// Facebook.
    /// </summary>
    public class UserSource
    {
        /// <summary>
        /// Gets or sets the name of the source, e.g. 'Mail', 'Facebook' or 'Twitter'.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of this source, e.g. the email address or username..
        /// </summary>
        public string Value { get; set; }
    }
}
