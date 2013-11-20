using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace nl.fhict.IntelliCloud.Business.DataTransfer
{
    /// <summary>
    /// A class representing a source type. A source can be a account for some service, like facebook, twitter or email.
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
        /// Gets or sets the name of the source, e.g. 'Facebook'.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a description of the source.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

    }
}