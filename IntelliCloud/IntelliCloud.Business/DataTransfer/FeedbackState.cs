using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nl.fhict.IntelliCloud.Business.DataTransfer
{
    /// <summary>
    /// An enumeration indicating the process state of the <see cref="Feedback"/>.
    /// </summary>
    [DataContract]
    public enum FeedbackState
    {
        /// <summary>
        /// Indicates the feedback is given but not processed.
        /// </summary>
        [EnumMember]
        Open,

        /// <summary>
        /// Indicates the feedback is processed and the answer updated.
        /// </summary>
        [EnumMember]
        Closed
    }
}
