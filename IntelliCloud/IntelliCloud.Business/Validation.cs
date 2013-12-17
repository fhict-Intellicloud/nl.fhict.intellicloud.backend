using System;
using System.Linq;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;

namespace nl.fhict.IntelliCloud.Business
{

    /// <summary>
    /// Class to validate incoming parameters.
    /// </summary>
    public class Validation : IValidation
    {

        /// <summary>
        /// Checks if the string is not null or empty.
        /// </summary>
        /// <param name="value">The string that needs to be checked</param>
        public void StringCheck(string value)
        {
            if (value == null)
                throw new ArgumentNullException("String is null.");
            else if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("String is empty.");
        }

        /// <summary>
        /// Checks if the id can be converted to an integer, and if it is postiive.
        /// </summary>
        /// <param name="value">The string that needs to be checked as an id.</param>
        public void IdCheck(string value)
        {
            int id;
            if (value == null)
            {
                throw new ArgumentNullException("Id has a null value");
            }
            else if (int.TryParse(value, out id))
            {
                if (id < 0)
                {
                    throw new ArgumentException("Id has to be positive.");
                }
            }
            else
            {
                throw new ArgumentException("Id can not be converted to an integer.");
            }
        }

        /// <summary>
        /// Checks if the id  is positive.
        /// </summary>
        /// <param name="value">The int that needs to be checked as an id.</param>
        public void IdCheck(int value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Id has to be positive.");
            }
        }

        /// <summary>
        /// Checks if the given SourceDefinitionName exists.
        /// </summary>
        /// <param name="SourceDefinitionName">SourceDefinitionName that has to be checked.</param>
        public void SourceDefinitionExistsCheck(string SourceDefinitionName)
        {
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {

                var sourceDefinitions = from sd in ctx.SourceDefinitions
                                        where sd.Name == SourceDefinitionName
                                        select sd;

                if (!(sourceDefinitions.Count() > 0))
                {
                    throw new ArgumentException("Source definition not found.");
                }
            }
        }

        /// <summary>
        /// Checks if a tweet is less or equal to 140 characters
        /// </summary>
        /// <param name="tweet">The answer to be send</param>
        public void TweetLengthCheck(string tweet)
        {
            if (String.IsNullOrWhiteSpace(tweet))
                throw new ArgumentException("Tweet is empty");
            else if (tweet.Length > 140)
                throw new ArgumentException("Tweet can't be longer then 140 characters");
        }
    }
}
