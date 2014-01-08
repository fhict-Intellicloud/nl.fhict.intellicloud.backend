using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nl.fhict.IntelliCloud.Common.DataTransfer
{
    /// <summary>
    /// A class representing a keyword that belongs to a user, answer or question.
    /// </summary>
    [DataContract]
    public class Keyword
    {
        /// <summary>
        /// Gets or sets the URL to the keywords that have a relation with an answer or question.
        /// </summary>
        public Uri Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the keyword. In this case the keyword itself.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the affinity of the keyword with the user, answer or question.
        /// </summary>
        [DataMember]
        public int Affinity { get; set; }

        /// <summary>
        /// Gets or sets the creation date and time of the relation between the user, answer or question and the 
        /// keyword.
        /// </summary>
        [DataMember]
        public DateTime CreationTime { get; set; }
    }
}
