﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace nl.fhict.IntelliCloud.Common.DataTransfer
{
    /// <summary>
    /// A class representing a review that is made about a specific <see cref="Answer"/>.
    /// </summary>
    [DataContract]
    public class Review
    {
        /// <summary>
        /// Gets or sets the URL to this specific review.
        /// </summary>
        [DataMember]
        public Uri Id { get; set; }

        /// <summary>
        /// Gets or sets the content of the review. The content contains the comments a user made about the answer.
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the URL to the answer that the review applies to.
        /// </summary>
        [DataMember]
        public Uri Answer { get; set; }

        /// <summary>
        /// Gets or sets the state of the review. This state indicates if the review is processed.
        /// </summary>
        public ReviewState ReviewState { get; set; }
        
        /// <summary>
        /// Gets or sets the URL to the user that made the review.
        /// </summary>
        [DataMember]
        public Uri User { get; set; }

        /// <summary>
        /// Gets or sets the creation date and time of the review.
        /// </summary>
        [DataMember]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the last changed date and time of the review.
        /// </summary>
        [DataMember]
        public DateTime? LastChangedTime { get; set; }
    }
}