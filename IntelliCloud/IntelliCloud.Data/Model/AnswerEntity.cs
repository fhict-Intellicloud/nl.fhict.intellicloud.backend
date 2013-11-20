using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using nl.fhict.IntelliCloud.Common.DataTransfer;

namespace nl.fhict.IntelliCloud.Data.Model
{
    /// <summary>
    /// A class representing the answer to a <see cref="Question"/>.
    /// </summary>
    [Table("Answer")]
    public class AnswerEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the answer.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the content of the answer. The content contains the answer given to the question.
        /// </summary>
        [Required]
        public string Content { get; set; }
        
        /// <summary>
        /// Gets or sets the question to which the answer applies.
        /// </summary>
        [Required]
        public QuestionEntity Question { get; set; }
        
        /// <summary>
        /// Gets or sets the user that gave the answer.
        /// </summary>
        [Required]
        public UserEntity User { get; set; }

        /// <summary>
        /// Gets or sets the state of the answer.
        /// </summary>
        public AnswerState AnswerState { get; set; }
    }
}