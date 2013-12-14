using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        /// Gets or sets the tile of the question. The title is a short summary of the question.
        /// </summary>
        [Required, MaxLength(256)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the content of the question. The content contains question asked by the <see cref="User"/>.
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the language the question is written in.
        /// </summary>
        [Required]
        public LanguageDefinitionEntity LanguageDefinition { get; set; }

        /// <summary>
        /// Gets or sets the answer for the question. This field is empty until the question state is either 
        /// <see cref="QuestionState.UpForFeedback"/> or <see cref="QuestionState.Closed"/>, so it only contains 
        /// accepted or pending answers.
        /// </summary>        
        public AnswerEntity Answer { get; set; }

        /// <summary>
        /// Gets or sets the user that asked the question.
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
        /// Gets or sets the creation date and time of the question.
        /// </summary>
        [Required]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the last changed date and time of the question.
        /// </summary>
        public DateTime? LastChangedTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the question is private. When a question is private it can only be 
        /// viewed by users of type <see cref="UserType.Employee"/>.
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Gets or sets the question source that is used to return the answer of the question. The question source 
        /// represents the <see cref="SourceEntity"/> the question originated from and extra information about where
        /// exactly the answer needs to be send.
        /// </summary>
        [Required]
        public QuestionSourceEntity Source { get; set; }

        /// <summary>
        /// Gets or sets the feedback token required to provide feedback to answers on this question. It is used to make
        /// sure the user that asked the question is also the user giving the feedback and that feedback can only be 
        /// given once.
        /// </summary>
        public string FeedbackToken { get; set; }
    }
}