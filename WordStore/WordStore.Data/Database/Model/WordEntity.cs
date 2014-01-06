using IntelliCloud.WordStore.Common.Word;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntelliCloud.WordStore.Data.Database.Model
{
    /// <summary>
    /// A database entity representing a <see cref="Word"/>.
    /// </summary>
    [Table("Word")]
    public class WordEntity
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WordEntity"/> class.
        /// </summary>
        public WordEntity()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The unique and auto-incremented identifier that is used for the word.
        /// </summary>
        [Key]
        public long WordId { get; set; }

        /// <summary>
        /// The value of the word, for instance 'bicycle'.
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// The type of the word, for instance 'Noun'.
        /// </summary>
        [Required]
        public WordType Type { get; set; }

        /// <summary>
        /// The language of the word, for instance 'Dutch'.
        /// </summary>
        [Required]
        public Language Language { get; set; }

        /// <summary>
        /// A collection containing all the keywords that correspond with this <see cref="WordEntity"/>.
        /// </summary>
        public ICollection<KeywordEntity> Keywords { get; set; }

        #endregion Properties
    }
}
