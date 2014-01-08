using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nl.fhict.IntelliCloud.Business
{
    /// <summary>
    /// Interface to validate incoming parameters.
    /// </summary>
    public interface IValidation
    {
        /// <summary>
        /// Checks if the string is not null or empty.
        /// </summary>
        /// <param name="value">The string that needs to be checked</param>
        void StringCheck(string value);

        /// <summary>
        /// Checks if the id can be converted to an integer, and if it is postiive.
        /// </summary>
        /// <param name="value">The string that needs to be checked as an id.</param>
        void IdCheck(string value);

        /// <summary>
        /// Checks if the id  is postiive.
        /// </summary>
        /// <param name="value">The int that needs to be checked as an id.</param>
        void IdCheck(int value);

        /// <summary>
        /// Checks if the given SourceDefinitionName exists.
        /// </summary>
        /// <param name="SourceDefinitionName">SourceDefinitionName that has to be checked.</param>
        void SourceDefinitionExistsCheck(string SourceDefinitionName);

        /// <summary>
        /// Checks if a tweet is less or equal to 140 characters
        /// </summary>
        /// <param name="tweet">The answer to be send</param>
        void TweetLengthCheck(string tweet);
    }
}
