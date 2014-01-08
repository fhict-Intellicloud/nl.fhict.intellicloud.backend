using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace nl.fhict.IntelliCloud.Data.IntelliCloud.Model
{
    /// <summary>
    /// A class representing the relation between a keyword and a user.
    /// </summary>
    [Table("UserKey")]
    public class UserKeyEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user keyword.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user the keyword applies to.
        /// </summary>
        [Required]
        public UserEntity User { get; set; }

        /// <summary>
        /// Gets or sets the keyword that is linked to the user.
        /// </summary>
        [Required]
        public KeywordEntity Keyword { get; set; }

        /// <summary>
        /// Gets or sets the affinity of the keyword with the user. The affinity is determined by the keyword amount of
        /// questions and answers the user processed.
        /// </summary>
        public int Affinity { get; set; }

        /// <summary>
        /// Gets or sets the creation date and time of the user keyword.
        /// </summary>
        [Required]
        public DateTime CreationTime { get; set; }
    }
}
