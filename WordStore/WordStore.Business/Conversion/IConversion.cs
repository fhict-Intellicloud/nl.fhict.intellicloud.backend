using IntelliCloud.WordStore.Common.Word;
using IntelliCloud.WordStore.Data.Database.Model;
using System.Collections.Generic;
using WordObject = IntelliCloud.WordStore.Common.Word.Word;

namespace IntelliCloud.WordStore.Business.Conversion
{
    /// <summary>
    /// An interface for a conversion object providing all functionality to convert objects.
    /// </summary>
    public interface IConversion
    {
        #region Methods

        /// <summary>
        /// Converts an instance of <see cref="WordEntity"/> to a corresponding instance of <see cref="WordObject"/>.
        /// </summary>
        /// <param name="wordEntity">The instance of <see cref="WordEntity"/> that must be converted.</param>
        /// <returns>The converted <see cref="WordEntity"/> as an instance of <see cref="WordObject"/>.</returns>
        WordObject ToWord(WordEntity wordEntity);

        /// <summary>
        /// Converts a string representing a keyword and a collection of <see cref="WordObject"/> instances 
        /// representing its matching words into an instance of <see cref="KeywordEntity"/>.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <param name="words">The collection of words that match the <paramref name="keyword"/>.</param>
        /// <returns>The converted <see cref="WordEntity"/> as an instance of <see cref="WordObject"/>.</returns>
        KeywordEntity ToKeywordEntity(string keyword, IEnumerable<WordObject> matchingWords);

        /// <summary>
        /// Converts a string representing a word type to its corresponding <see cref="WordType"/> value.
        /// </summary>
        /// <param name="wordType">The string representing a word type.</param>
        /// <returns>The corresponding <see cref="WordType"/> value.</returns>
        WordType ToWordType(string wordType);

        /// <summary>
        /// Converts a string representing a language into its corresponding value of <see cref="Language"/>.
        /// </summary>
        /// <param name="language">The string representing a language.</param>
        /// <returns>The corresponding value of <see cref="Language"/>.</returns>
        Language ToLanguage(string language);

        #endregion Methods
    }
}
