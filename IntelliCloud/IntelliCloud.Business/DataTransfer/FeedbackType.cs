using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nl.fhict.IntelliCloud.Business.DataTransfer
{
    /// <summary>
    /// An enumeration indicating if the answer was declined or accepted.
    /// </summary>
    [DataContract]
    public enum FeedbackType
    {
        /// <summary>
        /// Indicates that the answer was accepted.
        /// </summary>
        [EnumMember]
        Accepted,
        
        /// <summary>
        /// Indicates that the answer was declined.
        /// </summary>
        [EnumMember]
        Declined
    }
}
