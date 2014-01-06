using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntelliCloud.WordStore.Data.Database.Model
{
    /// <summary>
    /// A database entity representing a <see cref="Keyword"/>.
    /// </summary>
    [Table("Keyword")]
    public class KeywordEntity
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeywordEntity"/> class.
        /// </summary>
        public KeywordEntity()
        {
            this.Words = new List<WordEntity>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The unique and auto-incremented identifier that is used for the keyword.
        /// </summary>
        [Key]
        public long KeywordId { get; set; }
        
        /// <summary>
        /// The value of the word, for instance 'bicycle'.
        /// </summary>
        [Required, MaxLength(255)]
        public string Value { get; set; }

        /// <summary>
        /// A collection containing all the corresponding <see cref="WordEntity"/> objects.
        /// </summary>
        [ForeignKey("WordId")]
        public virtual ICollection<WordEntity> Words { get; set; }

        #endregion Properties
    }
}
