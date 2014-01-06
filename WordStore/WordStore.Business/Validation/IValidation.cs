using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliCloud.WordStore.Business.Validation
{
    /// <summary>
    /// An interface for a validation object providing all functionality to validate input parameters.
    /// </summary>
    public interface IValidation
    {
        /// <summary>
        /// Validates if a word is not equal to <c>null</c>, <c>string.Empty</c> or whitespace. In addition it is 
        /// verified that the string is a single word and thus does not contain spaces.
        /// </summary>
        /// <param name="word">The word that must be validated.</param>
        void ValidateWord(string word);
    }
}
