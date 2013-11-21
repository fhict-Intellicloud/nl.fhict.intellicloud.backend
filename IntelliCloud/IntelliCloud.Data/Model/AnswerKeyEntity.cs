using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace nl.fhict.IntelliCloud.Data.Model
{
    /// <summary>
    /// A class representing the relation between a keyword and answer.
    /// </summary>
    [Table("AnswerKey")]
    public class AnswerKeyEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the answer keyword.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the answer the keyword applies to.
        /// </summary>
        [Required]
        public AnswerEntity Answer { get; set; }
        
        /// <summary>
        /// Gets or sets the keyword that is linked to the answer.
        /// </summary>
        [Required]
        public KeywordEntity Keyword { get; set; }

        /// <summary>
        /// Gets or sets the affinity of the keyword with the answer. The affinity is determined by de count of the 
        /// keyword in the answer and by the amount of keywords in questions that have marked this answer as accepted or
        /// declined.
        /// </summary>
        public int Affinity { get; set; }
    }
}
