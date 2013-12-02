using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nl.fhict.IntelliCloud.Data.Model
{
    /// <summary>
    /// A class representing a language, e.g. 'Dutch' or 'English'.
    /// </summary>
    public class LanguageDefinitionEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the language definition.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the language as it is called in English, e.g. 'Dutch', 'German' or 'English'.
        /// </summary>
        [Required, MaxLength(254)]
        public string Name { get; set; }
    }
}
