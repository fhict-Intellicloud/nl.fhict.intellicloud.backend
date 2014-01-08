using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace nl.fhict.IntelliCloud.Common.DataTransfer
{
    /// <summary>
    /// A class representing a question that is made by a <see cref="User"/>.
    /// </summary>
    [DataContract]
    public class Question
    {
        /// <summary>
        /// Gets or sets the URL to this specific question.
        /// </summary>
        [DataMember]
        public Uri Id { get; set; }

        /// <summary>
        /// Gets or sets the tile of the question. The title is a short summary of the question.
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the content of the question. The content contains question asked by the <see cref="User"/>.
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the language the question is written in.
        /// </summary>
        [DataMember]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the URL to the answer for the question. This field is empty until the question state is either 
        /// <see cref="QuestionState.UpForFeedback"/> or <see cref="QuestionState.Closed"/>, so it only contains 
        /// accepted or pending answers.
        /// </summary>
        [DataMember]
        public Uri Answer { get; set; }

        /// <summary>
        /// Gets or sets the URL to the user that asked the question.
        /// </summary>
        [DataMember]
        public Uri User { get; set; }

        /// <summary>
        /// Gets or sets the user that is going to answer the question.
        /// </summary>
        [DataMember]
        public Uri Answerer { get; set; }

        /// <summary>
        /// Gets or sets the state of the question.
        /// </summary>
        public QuestionState QuestionState { get; set; }        

        /// <summary>
        /// Gets or sets the creation date and time of the question.
        /// </summary>
        [DataMember]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the last changed date and time of the question.
        /// </summary>
        [DataMember]
        public DateTime? LastChangedTime { get; set; }

        /// <summary>
        /// Gets or sets the URL to the keywords that are linked to the question.
        /// </summary>
        [DataMember]
        public Uri Keywords { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the question is private. When a question is private it can only be 
        /// viewed by users of type <see cref="UserType.Employee"/>.
        /// </summary>
        [DataMember]
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the post where the question originated from, like the Facebook post 
        /// identifier. This identifier is used so the answer can be replied to this post.
        /// </summary>
        [DataMember]
        public string SourcePostId { get; set; }

        /// <summary>
        /// Gets or sets the source that is used to return the answer of the question.
        /// </summary>
        [DataMember]
        public UserSource Source { get; set; }
    }
}