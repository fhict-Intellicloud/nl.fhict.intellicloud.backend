using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace nl.fhict.IntelliCloud.Business.DataTransfer
{
    /// <summary>
    /// A class representing a review that is made about a specific <see cref="Answer"/>.
    /// </summary>
    [DataContract]
    public class Review
    {
        /// <summary>
        /// Gets or sets the unique identifier of the review.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the content of the review. The content contains the comments a user made about the answer.
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the answer that the review applies to.
        /// </summary>
        [DataMember]
        public Answer Answer { get; set; }

        /// <summary>
        /// Gets or sets the state of the review. This state indicates if the review is processed.
        /// </summary>
        [DataMember]
        public ReviewState ReviewState { get; set; }

        /// <summary>
        /// Gets or sets the user that made the review.
        /// </summary>
        [DataMember]
        public User User { get; set; }

    }
}