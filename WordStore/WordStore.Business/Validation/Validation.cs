using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliCloud.WordStore.Business.Validation
{
    /// <summary>
    /// A validation class providing all functionality to validate input parameters.
    /// </summary>
    public class Validation : IValidation
    {
        /// <summary>
        /// Validates if a word is not equal to <c>null</c>, <c>string.Empty</c> or whitespace. In addition it is 
        /// verified that the string is a single word and thus does not contain spaces.
        /// </summary>
        /// <param name="word">The word that must be validated.</param>
        public void ValidateWord(string word)
        {
            if (word == null)
                throw new ArgumentNullException("The word cannot be equal to null");
            else if (string.IsNullOrWhiteSpace(word))
                throw new ArgumentOutOfRangeException("The word cannot be an empty string or only contain whitespace characters.");
            else if (word.Split(' ').Length != 1)
                throw new ArgumentOutOfRangeException("The word must consist of a single word without spaces.");
        }
    }
}
