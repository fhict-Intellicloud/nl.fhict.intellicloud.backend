using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nl.fhict.IntelliCloud.Common.DataTransfer
{
    /// <summary>
    /// A class representing the <see cref="Source"/> instances of a <see cref="User"/>.
    /// </summary>
    [DataContract]
    public class Source
    {
        /// <summary>
        /// Gets or sets the unique identifier of the source.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the definition of the source for which a value is provided, e.g. email, facebook or twitter.
        /// </summary>
        [DataMember]
        public SourceDefinition SourceDefinition { get; set; }

        /// <summary>
        /// Gets or sets the value for the for the given source definition, e.g. an email address or username.
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the creation date and time of the answer.
        /// </summary>
        [DataMember]
        public DateTime CreationTime { get; set; }
    }
}
