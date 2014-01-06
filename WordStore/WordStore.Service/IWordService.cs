using IntelliCloud.WordStore.Common.Word;
using System.Collections.Generic;
using System.ServiceModel;

namespace IntelliCloud.WordStore.Service
{
    /// <summary>
    /// An interface for a service providing functionality to resolve a string into a list of actual words including 
    /// its language and type.
    /// </summary>
    [ServiceContract]
    public interface IWordService
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
        [OperationContract]
        IEnumerable<Word> ResolveWord(string word);
    }
}
