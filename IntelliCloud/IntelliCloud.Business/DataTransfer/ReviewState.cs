using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nl.fhict.IntelliCloud.Business.DataTransfer
{
    /// <summary>
    /// An enumeration indicating the states of a <see cref="Review"/>.
    /// </summary>
    [DataContract]
    public enum ReviewState
    {
        /// <summary>
        /// Indicates the review is made but not jet implemented in the answer.
        /// </summary>
        [EnumMember]
        Open,

        /// <summary>
        /// Indicates the review is implemented in the answer.
        /// </summary>
        [EnumMember]
        Closed
    }
}
