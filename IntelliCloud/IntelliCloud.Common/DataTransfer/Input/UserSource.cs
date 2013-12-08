namespace nl.fhict.IntelliCloud.Common.DataTransfer.Input
{
    /// <summary>
    /// Class used when creating or updating a user and it's sources.
    /// </summary>
    public class UserSource
    {
        /// <summary>
        /// Gets or sets the name of the SourceDefinition that is used for this source.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of this source.
        /// </summary>
        public string Value { get; set; }
    }
}
