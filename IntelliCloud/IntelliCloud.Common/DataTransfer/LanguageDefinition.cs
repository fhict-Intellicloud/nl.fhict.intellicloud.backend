using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nl.fhict.IntelliCloud.Common.DataTransfer
{
    /// <summary>
    /// A class representing a language, e.g. 'Dutch' or 'English'.
    /// </summary>
    [DataContract]
    public class LanguageDefinition
    {
        /// <summary>
        /// Gets or sets the unique identifier of the language definition.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the language as it is called in English, e.g. 'Dutch', 'German' or 'English'.
        /// </summary>
        [DataMember]
        public string Name { get; set; }
    }
}
