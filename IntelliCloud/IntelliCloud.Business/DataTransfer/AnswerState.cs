using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nl.fhict.IntelliCloud.Business.DataTransfer
{
    /// <summary>
    /// An enumeration indicating the state of the <see cref="Answer"/>.
    /// </summary>
    [DataContract]
    public enum AnswerState
    {
        /// <summary>
        /// A state indicating that the <see cref="Answer"/> is send to the <see cref="User"/> that aksed the <see cref="Question"/> that is answered.
        /// </summary>
        [EnumMember]
        Send,

        /// <summary>
        /// A state indicating that the <see cref="Answer"/> is under review.
        /// </summary>
        [EnumMember]
        UnderReview,

        /// <summary>
        /// A state indicating that the <see cref="Answer"/> is accepted by the <see cref="User"/> that received the answer. This state can only be applied after the answer is send.
        /// </summary>
        [EnumMember]
        Accepted,

        /// <summary>
        /// A state indicating that the <see cref="Answer"/> is declined by the <see cref="User"/> that received the answer. This state can only be applied after the answer is send.
        /// </summary>
        [EnumMember]
        Declined
    }
}
