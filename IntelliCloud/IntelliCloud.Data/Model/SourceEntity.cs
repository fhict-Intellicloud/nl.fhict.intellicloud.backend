using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nl.fhict.IntelliCloud.Data.Model
{
    /// <summary>
    /// A class representing the <see cref="Source"/> instances of a <see cref="User"/>.
    /// </summary>
    [Table("Source")]
    public class SourceEntity
    {
        /// <summary>
        /// Gets or sets the unique indentifier of the source.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the reference to the user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the definition of the source for which a value is provided, e.g. email, facebook or twitter.
        /// </summary>
        [Required]
        public SourceDefinitionEntity SourceDefinition { get; set; }

        /// <summary>
        /// Gets or sets the value for the for the given source definition, e.g. an email address or username.
        /// </summary>
        [Required, MaxLength(254)]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the creation date and time of the source.
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
