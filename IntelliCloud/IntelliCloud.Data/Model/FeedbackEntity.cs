using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using nl.fhict.IntelliCloud.Common.DataTransfer;

namespace nl.fhict.IntelliCloud.Data.Model
{
    /// <summary>
    /// A class representing the feedback that is given on a <see cref="Answer"/>.
    /// </summary>
    [Table("Feedback")]
    public class FeedbackEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the feedback.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the content of the feedback. The content represents the feedback the customer gave.
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the answer the feedback applies to.
        /// </summary>
        [Required]
        public AnswerEntity Answer { get; set; }

        /// <summary>
        /// Gets or sets the question which was answered. This field is required because the answer only has a link to
        /// the question that triggered its creation and the question only has a link to the accepted or pending answer.
        /// This feedback however can also apply to existing answers (so this question wasn't the question that 
        /// triggered its creation) that where declined for this question (so the answer isn't linked to this question).
        /// </summary>
        [Required]
        public QuestionEntity Question { get; set; }

        /// <summary>
        /// Gets or sets the user that gave the feedback.
        /// </summary>        
        public UserEntity User { get; set; }

        /// <summary>
        /// Gets or sets the type of feedback. The type indicates if the answer was accepted or declined.
        /// </summary>
        public FeedbackType FeedbackType { get; set; }

        /// <summary>
        /// Gets or sets the state of the feedback. The state indicates if the feedback is already processed.
        /// </summary>
        public FeedbackState FeedbackState { get; set; }

        /// <summary>
        /// Gets or sets the creation date and time of the feedback.
        /// </summary>
        [Required]
        public DateTime CreationTime { get; set; }
    }
}
