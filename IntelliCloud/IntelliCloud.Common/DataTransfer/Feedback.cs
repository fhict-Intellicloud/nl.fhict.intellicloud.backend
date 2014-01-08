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
        /// Gets or sets the URL to this specific feedback item.
        /// </summary>
        [DataMember]
        public Uri Id { get; set; }

        /// <summary>
        /// Gets or sets the content of the feedback. The content represents the feedback the customer gave.
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the URL to the answer the feedback applies to.
        /// </summary>
        [DataMember]
        public Uri Answer { get; set; }

        /// <summary>
        /// Gets or sets the URL to the question which was answered. This field is required because the answer 
        /// only has a link to the question that triggered its creation and the question only has a link to the accepted
        /// or pending answer. This feedback however can also apply to existing answers (so this question wasn't the 
        /// question that triggered its creation) that where declined for this question (so the answer isn't linked to
        /// this question).
        /// </summary>
        [DataMember]
        public Uri Question { get; set; }

        /// <summary>
        /// Gets or sets the URL to the user that gave the feedback.
        /// </summary>
        [DataMember]
        public Uri User { get; set; }

        /// <summary>
        /// Gets or sets the type of feedback. The type indicates if the answer was accepted or declined.
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

        /// <summary>
        /// Gets or sets the last changed date and time of the feedback.
        /// </summary>
        [DataMember]
        public DateTime? LastChangedTime { get; set; }
    }
}
