using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using nl.fhict.IntelliCloud.Common.DataTransfer;

namespace nl.fhict.IntelliCloud.Data.Model
{
    /// <summary>
    /// A class representing a question that is made by a <see cref="User"/>.
    /// </summary>
    [Table("Question")]
    public class QuestionEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the question.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the content of the question. The content contains question asked by the <see cref="User"/>.
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the user that aksed the question.
        /// </summary>
        [Required]
        public UserEntity User { get; set; }

        /// <summary>
        /// Gets or sets the user that is going to answer the question.
        /// </summary>
        public UserEntity Answerer { get; set; }

        /// <summary>
        /// Gets or sets the state of the question.
        /// </summary>
        public QuestionState QuestionState { get; set; }

        /// <summary>
        /// Gets or sets the type of source that is used to return the answer to the question. The actual source can be found using the <see cref="Question.User"/> field.
        /// </summary>
        [Required]
        public SourceDefinitionEntity SourceType { get; set; }

    }
}