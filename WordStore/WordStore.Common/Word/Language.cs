using System.Runtime.Serialization;

namespace IntelliCloud.WordStore.Common.Word
{
    /// <summary>
    /// An enum used to indicate the language.
    /// </summary>
    [DataContract]
    public enum Language
    {
        /// <summary>
        /// Indicates that the language is 'English', note that this also implies 'American'.
        /// </summary>
        [EnumMember]
        English,

        /// <summary>
        /// Indicates that the language is 'Dutch'.
        /// </summary>
        [EnumMember]
        Dutch,

        /// <summary>
        /// Indicates that the language could not be determined and is 'Unknown'.
        /// </summary>
        [EnumMember]
        Unknown,
    }
}
