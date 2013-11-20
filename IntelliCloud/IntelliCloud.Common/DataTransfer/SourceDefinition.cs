using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace nl.fhict.IntelliCloud.Common.DataTransfer
{
    /// <summary>
    /// A class representing the definition of a source. A source can be a account for some service, like facebook, 
    /// twitter or email.
    /// </summary>
    [DataContract]
    public class SourceDefinition
    {
        /// <summary>
        /// Gets or sets the unique identifier of the source definition.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the source definition, e.g. 'Facebook'.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a description of the source definition.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

    }
}