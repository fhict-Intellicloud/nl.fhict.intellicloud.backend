using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace nl.fhict.IntelliCloud.Business.DataTransfer
{
    /// <summary>
    /// A class representing a question that is made by a <see cref="User"/>.
    /// </summary>
    [DataContract]
    public class Question
    {
        /// <summary>
        /// Gets or sets the unique identifier of the question.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the content of the question. The content contains question asked by the <see cref="User"/>.
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the user that aksed the question.
        /// </summary>
        [DataMember]
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the user that is going to answer the question.
        /// </summary>
        [DataMember]
        public User Answerer { get; set; }

        /// <summary>
        /// Gets or sets the state of the question.
        /// </summary>
        [DataMember]
        public QuestionState QuestionState { get; set; }

        /// <summary>
        /// Gets or sets the type of source that is used to return the answer to the question. The actual source can be found using the <see cref="Question.User"/> field.
        /// </summary>
        [DataMember]
        public Source SourceType { get; set; }

    }
}