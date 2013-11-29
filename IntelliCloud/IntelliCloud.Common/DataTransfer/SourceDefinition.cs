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
        /// Gets or sets the name of the source definition, e.g. 'Facebook', 'Twitter', 'Mail' or a openId issuer.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a description of the source definition.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the url that can be used to retrieve information about a user. The url is used to send a token
        /// to an external system to verify its authenticity. It is mainly used by openId providers.
        /// </summary>
        [DataMember]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the creation date and time of the answer.
        /// </summary>
        [DataMember]
        public DateTime CreationTime { get; set; }

    }
}