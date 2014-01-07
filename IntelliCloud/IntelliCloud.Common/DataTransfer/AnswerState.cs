using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nl.fhict.IntelliCloud.Common.DataTransfer
{
    /// <summary>
    /// An enumeration indicating the state of the <see cref="Answer"/>.
    /// </summary>
    [DataContract]
    public enum AnswerState
    {
        /// <summary>
        /// A state indicating that the <see cref="Answer"/> is ready to be used by the system. This means the system
        /// can used it to automatically answer questions and employees can use it to manually send it.
        /// </summary>
        [EnumMember]
        Ready,

        /// <summary>
        /// A state indicating that the <see cref="Answer"/> is under review. This means the answer cannot be send to
        /// customers.
        /// </summary>
        [EnumMember]
        UnderReview
    }
}
