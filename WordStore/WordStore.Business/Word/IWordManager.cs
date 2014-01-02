using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordObject = IntelliCloud.WordStore.Common.Word.Word;

namespace IntelliCloud.WordStore.Business.Word
{
    /// <summary>
    /// An interface for a business manager providing functionality to resolve a string into a list of actual words 
    /// including its language and type.
    /// </summary>
    public interface IWordManager
    {
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
        IEnumerable<WordObject> ResolveWord(string word);
    }
}
