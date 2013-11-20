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
    /// A class representing a review that is made about a specific <see cref="Answer"/>.
    /// </summary>
    [Table("Review")]
    public class ReviewEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the review.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the content of the review. The content contains the comments a user made about the answer.
        /// </summary>
        [Required, MaxLength(254)]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the answer that the review applies to.
        /// </summary>
        [Required]
        public AnswerEntity Answer { get; set; }

        /// <summary>
        /// Gets or sets the state of the review. This state indicates if the review is processed.
        /// </summary>
        public ReviewState ReviewState { get; set; }

        /// <summary>
        /// Gets or sets the user that made the review.
        /// </summary>
        [Required]
        public UserEntity User { get; set; }
    }
}