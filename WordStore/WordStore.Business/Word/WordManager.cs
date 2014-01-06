using HtmlAgilityPack;
using IntelliCloud.WordStore.Business.Conversion;
using IntelliCloud.WordStore.Business.Validation;
using IntelliCloud.WordStore.Common.Word;
using IntelliCloud.WordStore.Data.Database.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WordObject = IntelliCloud.WordStore.Common.Word.Word;

namespace IntelliCloud.WordStore.Business.Word
{
    /// <summary>
    /// A business manager providing functionality to resolve a string into a list of actual words including its 
    /// language and type.
    /// </summary>
    public class WordManager : IWordManager
    {
        #region Fields

        /// <summary>
        /// A field containing an instance providing the required conversion functionality.
        /// </summary>
        private readonly IConversion conversion;

        /// <summary>
        /// A field containing an instance providing the required validation functionality to validate all the input
        /// parameters.
        /// </summary>
        private readonly IValidation validation;

        /// <summary>
        /// A factory that is used to create new instances of <see cref="IWordStoreContext"/>.
        /// </summary>
        private readonly IWordStoreContextFactory wordStoreContextFactory;

        /// <summary>
        /// An instance that is used to perform HTTP requests and to retrieve the replied HTML documents.
        /// </summary>
        private readonly HtmlWeb htmlWeb;

        /// <summary>
        /// A regular expression that is used to parse the HTML element containing the required word information from
        /// the dictionary website.
        /// </summary>
        private readonly Regex wordElementRegex;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WordManager"/> class.
        /// </summary>
        public WordManager()
            : this(new Conversion.Conversion(), new Validation.Validation(), new WordStoreContextFactory())
        {
            this.htmlWeb = new HtmlWeb();
            this.wordElementRegex = new Regex(
                "^<span class=\"babFlag babFlag-(?<Language>\\w{2})\"></span>" 
                + "(?<Value>[\\w ]+)"
                + "<span class=\"muted\"> {(?<Type>[\\w| ]+)}</span>$");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WordManager"/> class.
        /// </summary>
        /// <param name="conversion">The object responsible for performing conversion functionality.</param>
        /// <param name="validation">The object responsible for performing validation functionality.</param>
        /// <param name="wordStoreContextFactory">The factory responsible for providing new 
        /// <see cref="IWordStoreContext"/> objects.</param>
        internal WordManager(
            IConversion conversion, IValidation validation, IWordStoreContextFactory wordStoreContextFactory)
        {
            this.conversion = conversion;
            this.validation = validation;
            this.wordStoreContextFactory = wordStoreContextFactory;
        }

        #endregion Constructors

        #region Methods

        #region Public

        /// <summary>
        /// Resolves a string that represents a word into all occurrences of this word in both the dutch and english 
        /// dictionary. For instance, the word 'pen' will resolve to:
        /// <ul>
        ///     <li>The dutch noun 'pen'</li>
        ///     <li>The english noun 'pen'</li>
        ///     <li>The english verb 'to pen'</li>
        /// </ul>
        /// </summary>
        /// <param name="word">The word that must be resolved.</param>
        /// <returns>The instances of <see cref="Word"/> associated with the given <paramref name="word"/>.</returns>
        /// <remarks>Note that when a word could not be found in any of the available dictionaries, a collection with 
        /// a single instance of <see cref="Word"/> will be returned of type <see cref="WordType.Unknown"/> and 
        /// language <see cref="Language.Unknown"/>.</remarks>
        public IEnumerable<WordObject> ResolveWord(string word)
        {
            this.validation.ValidateWord(word);

            var wordsInDatabase = this.GetWordsFromDatabase(word);
            if (wordsInDatabase.Any())
                return wordsInDatabase;
            else
            {
                var wordsFromWeb = this.GetWordsFromWeb(word);
                var matchingWords = wordsFromWeb.Any() 
                    ? wordsFromWeb 
                    : new[] { new WordObject(word, WordType.Unknown, Language.Unknown) };

                this.AddWordsToDatabase(word, matchingWords);

                return matchingWords;
            }
        }

        #endregion Public

        #region Private

        /// <summary>
        /// Retrieves the words that can be associated with the given <paramref name="word"/> by querying an online
        /// dictionary on the web.
        /// </summary>
        /// <param name="word">The word for which the corresponding <see cref="Word"/> objects must be retrieved.
        /// </param>
        /// <returns>The collection of <see cref="Word"/> objects that correspond to the given <see cref="Word"/> in 
        /// both english as dutch.</returns>
        private IEnumerable<WordObject> GetWordsFromWeb(string word)
        {
            HtmlDocument doc = this.htmlWeb.Load(string.Format("http://nl.bab.la/woordenboek/engels-nederlands/{0}", word));

            try
            {
                var wordElements = doc.DocumentNode
                    .ChildNodes.Single(x => x.Name == "html")
                    .ChildNodes.Single(x => x.Id == "babBody")
                    .ChildNodes.Single(x => x.Id == "page")
                    .ChildNodes.Single(x => x.Id == "main")
                    .ChildNodes.First(x => x.Name == "div")
                    .ChildNodes.Last(x => x.Name == "div")
                    .ChildNodes.First(x => x.Name == "section")
                    .ChildNodes.Where(x => x.Name == "h3")
                    .Select(x => x.InnerHtml)
                    .Distinct();

                return wordElements
                    .Select(x => this.wordElementRegex.Match(x))
                    .Where(x => x.Success)
                    .Select(x => new WordObject(
                        value: x.Groups["Value"].Value,
                        type: this.conversion.ToWordType(x.Groups["Type"].Value),
                        language: this.conversion.ToLanguage(x.Groups["Language"].Value)));
            }
            catch (Exception)
            {
                return Enumerable.Empty<WordObject>();
            }
        }

        /// <summary>
        /// Retrieves the words that can be associated with the given <paramref name="word"/> by querying the database.
        /// </summary>
        /// <param name="word">The word for which the corresponding <see cref="Word"/> objects must be retrieved.
        /// </param>
        /// <returns>The collection of <see cref="Word"/> objects that correspond to the given <see cref="Word"/> in 
        /// both english as dutch.</returns>
        private IEnumerable<WordObject> GetWordsFromDatabase(string word)
        {
            using (var context = this.wordStoreContextFactory.Create())
            {
                var keywordEntity = context.Keywords.SingleOrDefault(x => x.Value == word);
                if (keywordEntity != null && keywordEntity.Words.Any())
                    return keywordEntity.Words.Select(x => this.conversion.ToWord(x));
                else
                    return Enumerable.Empty<WordObject>();
            }
        }

        /// <summary>
        /// Populates the database with a new instance of <see cref="KeywordEntity"/> containing the given words as 
        /// <see cref="WordEntity"/> objects.
        /// </summary>
        /// <param name="keyword">The keyword that must be added to the database.</param>
        /// <param name="matchingWords">The <see cref="Word"/> objects that can be associated with the 
        /// <paramref name="keyword"/>.</param>
        private void AddWordsToDatabase(string keyword, IEnumerable<WordObject> matchingWords)
        {
            using (var context = this.wordStoreContextFactory.Create())
            {
                context.Keywords.Add(
                    this.conversion.ToKeywordEntity(
                        keyword: keyword,
                        matchingWords: matchingWords));

                context.SaveChanges();
            }
        }

        #endregion Private

        #endregion Methods
    }
}
