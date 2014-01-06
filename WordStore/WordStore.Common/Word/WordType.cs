using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IntelliCloud.WordStore.Common.Word
{
    /// <summary>
    /// An enum used to indicate of which type a word is.
    /// </summary>
    [DataContract]
    public enum WordType
    {
        /// <summary>
        /// Indicates that the word is a verb. (Werkwoord)
        /// </summary>
        [EnumMember]
        Verb,

        /// <summary>
        /// Indicates that the word is a noun. (Zelfstandig naamwoord)
        /// </summary>
        [EnumMember]
        Noun,

        /// <summary>
        /// Indicates that the word is an interjection. (Tussenwoord)
        /// </summary>
        [EnumMember]
        Interjection,

        /// <summary>
        /// Indicates that the word is an adverb. (Bijwoord)
        /// </summary>
        [EnumMember]
        Adverb,

        /// <summary>
        /// Indicates that the word is an adjective. (Bijvoeglijk naamwoord)
        /// </summary>
        [EnumMember]
        Adjective,

        /// <summary>
        /// Indicates that the word is a pronoun. (Voornaamwoord)
        /// </summary>
        [EnumMember]
        Pronoun,

        /// <summary>
        /// Indicates that the word is an article. (Lidwoord)
        /// </summary>
        [EnumMember]
        Article,

        /// <summary>
        /// Indicates that the type of the word is unknown.
        /// </summary>
        [EnumMember]
        Unknown,
    }
}
