using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nl.fhict.IntelliCloud.Common.DataTransfer
{
    /// <summary>
    /// An enumeration indicating the state of a <see cref="Question"/>.
    /// </summary>
    [DataContract]
    public enum QuestionState
    {
        /// <summary>
        /// Indicates the <see cref="Question"/> is received.
        /// </summary>
        [EnumMember]
        Open,
        
        /// <summary>
        /// Indicates the <see cref="Question"/> is assigned to an employee to be answered.
        /// </summary>
        [EnumMember]
        UpForAnswer,

        /// <summary>
        /// Indicates the <see cref="Answer"/> for the <see cref="Question"/> is send but it isn't confirmed if it was accepted.
        /// </summary>
        [EnumMember]
        UpForFeedback,

        /// <summary>
        /// Indicates the <see cref="Question"/> is answered and the answer is accepted.
        /// </summary>
        [EnumMember]
        Closed
    }
}
