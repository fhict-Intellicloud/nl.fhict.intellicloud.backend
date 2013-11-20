using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nl.fhict.IntelliCloud.Business.DataTransfer
{
    /// <summary>
    /// A class representing the <see cref="Source"/> instances of a <see cref="User"/>.
    /// </summary>
    [DataContract]
    public class SourceUser
    {
        /// <summary>
        /// Gets or sets the unique indentifier of a user source.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the source for which a value is provided, e.g. email, facebook or twitter.
        /// </summary>
        [DataMember]
        public Source Source { get; set; }

        /// <summary>
        /// Gets or sets the value for the for the given source, e.g. an email address or username.
        /// </summary>
        [DataMember]
        public string Value { get; set; }
    }
}
