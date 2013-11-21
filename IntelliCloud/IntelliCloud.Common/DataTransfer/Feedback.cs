using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nl.fhict.IntelliCloud.Common.DataTransfer
{
    /// <summary>
    /// A class representing the feedback that is given on a <see cref="Answer"/>.
    /// </summary>
    [DataContract]
    public class Feedback
    {
        /// <summary>
        /// Gets or sets the content of the feedback. The content represents the feedback the customer gave.
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the answer the feedback applies to.
        /// </summary>
        [DataMember]
        public int AnswerId { get; set; }

        /// <summary>
        /// Gets or sets the user that gave the feedback.
        /// </summary>
        [DataMember]
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the type of feedback. The type indicates if the anser was accepted or declined.
        /// </summary>
        [DataMember]
        public FeedbackType FeedbackType { get; set; }

        /// <summary>
        /// Gets or sets the state of the feedback. The state indicates if the feedback is already processed.
        /// </summary>
        [DataMember]
        public FeedbackState FeedbackState { get; set; }

        /// <summary>
        /// Gets or sets the creation date and time of the feedback.
        /// </summary>
        [DataMember]
        public DateTime CreationTime { get; set; }
    }
}
