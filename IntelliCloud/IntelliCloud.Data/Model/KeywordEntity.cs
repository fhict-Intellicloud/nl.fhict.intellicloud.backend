using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace nl.fhict.IntelliCloud.Data.Model
{
    /// <summary>
    /// A class representing a keyword that can be used to match questions and answers. A keyword is an word that 
    /// describes a part of a question or answer.
    /// </summary>
    [Table("Keyword")]
    public class KeywordEntity
    {

        /// <summary>
        /// Gets or sets the unique identifier of the keyword.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the keyword. In this case the keyword itself.
        /// </summary>
        [Required, MaxLength(254)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the creation date and time of the keyword.
        /// </summary>
        [Required]
        public DateTime CreationTime { get; set; }
    }
}
