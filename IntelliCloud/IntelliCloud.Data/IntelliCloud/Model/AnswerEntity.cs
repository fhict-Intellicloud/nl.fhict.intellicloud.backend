using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using nl.fhict.IntelliCloud.Common.DataTransfer;

namespace nl.fhict.IntelliCloud.Data.IntelliCloud.Model
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
        /// Gets or sets the language the answer is written in.
        /// </summary>
        [Required]
        public LanguageDefinitionEntity LanguageDefinition { get; set; }
        
        /// <summary>
        /// Gets or sets the user that gave the answer.
        /// </summary>
        [Required]
        public UserEntity User { get; set; }

        /// <summary>
        /// Gets or sets the state of the answer.
        /// </summary>
        public AnswerState AnswerState { get; set; }

        /// <summary>
        /// Gets or sets the creation date and time of the answer.
        /// </summary>
        [Required]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the last changed date and time of the answer.
        /// </summary>
        public DateTime? LastChangedTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the answer is private. When a answer is private it can only be 
        /// viewed by users of type <see cref="UserType.Employee"/>.
        /// </summary>
        public bool IsPrivate { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the original <see cref="Answer"/>, e.g. the first version of the answer. This
        /// identifier is required so the different versions of an answer can be easily retrieved. When this answer is 
        /// the first version of the answer the value is <c>null</c>.
        /// </summary>
        public int? OriginalId { get; set; }

        /// <summary>
        /// Gets or sets the date time which marks the time from which this answer is valid.
        /// </summary>
        [Required]
        public DateTime ValidFrom { get; set; }

        /// <summary>
        /// Gets or sets the date time which marks the time after this answer is no longer valid, e.g. a newer version 
        /// is available. When the record is valid this value is <see cref="DateTime.MaxValue"/>.
        /// </summary>
        [Required]
        public DateTime ValidTo { get; set; }
    }
}