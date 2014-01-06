using IntelliCloud.WordStore.Common.Word;
using IntelliCloud.WordStore.Data.Database.Model;
using System.Collections.Generic;
using System.Linq;
using WordObject = IntelliCloud.WordStore.Common.Word.Word;

namespace IntelliCloud.WordStore.Business.Conversion
{
    /// <summary>
    /// A conversion class providing all functionality to convert objects.
    /// </summary>
    public class Conversion : IConversion
    {
        #region Methods

        #region Public

        /// <summary>
        /// Converts an instance of <see cref="WordEntity"/> to a corresponding instance of <see cref="WordObject"/>.
        /// </summary>
        /// <param name="wordEntity">The instance of <see cref="WordEntity"/> that must be converted.</param>
        /// <returns>The converted <see cref="WordEntity"/> as an instance of <see cref="WordObject"/>.</returns>
        public WordObject ToWord(WordEntity wordEntity)
        {
            return new WordObject(
                value: wordEntity.Value,
                type: wordEntity.Type,
                language: wordEntity.Language);
        }

        /// <summary>
        /// Converts a string representing a word type to its corresponding <see cref="WordType"/> value.
        /// </summary>
        /// <param name="wordType">The string representing a word type.</param>
        /// <returns>The corresponding <see cref="WordType"/> value.</returns>
        public WordType ToWordType(string wordType)
        {
            switch (wordType.ToLowerInvariant())
            {
                case "bijvoeglijk naamwoord":
                    return WordType.Adjective;
                case "werkwoord":
                    return WordType.Verb;
                case "zelfstandig naamwoord":
                    return WordType.Noun;
                case "tussenwoord":
                    return WordType.Interjection;
                case "bijwoord":
                    return WordType.Adverb;
                case "voornaamwoord":
                    return WordType.Pronoun;
                case "lidwoord":
                    return WordType.Article;
                default:
                    return WordType.Unknown;
            }
        }

        /// <summary>
        /// Converts a string representing a language into its corresponding value of <see cref="Language"/>.
        /// </summary>
        /// <param name="language">The string representing a language.</param>
        /// <returns>The corresponding value of <see cref="Language"/>.</returns>
        public Language ToLanguage(string language)
        {
            switch (language.Trim().ToLowerInvariant())
            {
                case "nl":
                    return Language.Dutch;
                case "uk":
                    return Language.English;
                case "us":
                    return Language.English;
                default:
                    return Language.Unknown;
            }
        }

        /// <summary>
        /// Converts a string representing a keyword and a collection of <see cref="WordObject"/> instances 
        /// representing its matching words into an instance of <see cref="KeywordEntity"/>.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <param name="words">The collection of words that match the <paramref name="keyword"/>.</param>
        /// <returns>The converted <see cref="WordEntity"/> as an instance of <see cref="WordObject"/>.</returns>
        public KeywordEntity ToKeywordEntity(string keyword, IEnumerable<WordObject> words)
        {
            return new KeywordEntity()
            {
                Value = keyword,
                Words = words.Select(x => this.ToWordEntity(x)).ToList(),
            };
        }

        #endregion Public

        #region Private

        /// <summary>
        /// Converts an instance of <see cref="WordObject"/> to a corresponding instance of <see cref="WordEntity"/>.
        /// </summary>
        /// <param name="word">The instance of <see cref="WordObject"/> that must be converted.</param>
        /// <returns>The converted <see cref="WordObject"/> as an instance of <see cref="WordEntity"/>.</returns>
        private WordEntity ToWordEntity(WordObject word)
        {
            return new WordEntity()
            {
                Language = word.Language,
                Type = word.Type,
                Value = word.Value,
            };
        }

        #endregion Private

        #endregion Methods
    }
}
