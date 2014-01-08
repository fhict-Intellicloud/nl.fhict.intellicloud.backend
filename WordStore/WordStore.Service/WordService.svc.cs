using IntelliCloud.WordStore.Business.Word;
using IntelliCloud.WordStore.Common.Word;
using System.Collections.Generic;

namespace IntelliCloud.WordStore.Service
{
    /// <summary>
    /// A service providing functionality to resolve a string into a list of actual words including its language and 
    /// type.
    /// </summary>
    public class WordService : IWordService
    {
        #region Fields

        /// <summary>
        /// A field containing the business manager that is used to perform all business logic with respect to the
        /// <see cref="IWordService"/>.
        /// </summary>
        private IWordManager manager;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WordService"/> class.
        /// </summary>
        public WordService()
            : this(new WordManager())
        {
            this.manager = new WordManager();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WordService"/> class.
        /// </summary>
        /// <param name="manager">The manager responsible for performing all business logic with respect to the 
        /// <see cref="IWordService"/>.</param>
        internal WordService(IWordManager manager)
        {
            this.manager = manager;
        }

        #endregion Constructors

        #region Methods

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
        public IEnumerable<Word> ResolveWord(string word)
        {
            return this.manager.ResolveWord(word);
        }

        #endregion methods
    }
}
