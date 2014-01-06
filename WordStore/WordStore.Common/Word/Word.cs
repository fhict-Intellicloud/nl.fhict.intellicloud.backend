using System.Runtime.Serialization;

namespace IntelliCloud.WordStore.Common.Word
{
    /// <summary>
    /// A class representing an actual word.
    /// </summary>
    [DataContract]
    public class Word
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Word"/> class.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="language"></param>
        public Word(string value, WordType type, Language language)
        {
            this.Value = value;
            this.Type = type;
            this.Language = language;
        }

        /// <summary>
        /// The type of this word.
        /// </summary>
        [DataMember]
        public WordType Type { get; set; }

        /// <summary>
        /// The language of this word.
        /// </summary>
        [DataMember]
        public Language Language { get; set; }

        /// <summary>
        /// The value of this word, for instance 'bicycle'.
        /// </summary>
        [DataMember]
        public string Value { get; set; }
    }
}
