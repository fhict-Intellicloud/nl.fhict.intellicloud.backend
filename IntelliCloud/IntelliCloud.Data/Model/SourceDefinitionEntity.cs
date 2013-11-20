using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace nl.fhict.IntelliCloud.Data.Model
{
    /// <summary>
    /// A class representing the definition of a source. A source can be a account for some service, like facebook, 
    /// twitter or email.
    /// </summary>
    [Table("SourceDefinition")]
    public class SourceDefinitionEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the source definition.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the source definition, e.g. 'Facebook'.
        /// </summary>
        [Required, MaxLength(254)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a description of the source definition.
        /// </summary>
        [MaxLength(254)]
        public string Description { get; set; }

    }
}