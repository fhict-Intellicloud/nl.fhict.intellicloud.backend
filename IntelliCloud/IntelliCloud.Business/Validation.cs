using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;

namespace nl.fhict.IntelliCloud.Business
{
    public class Validation
    {

        /// <summary>
        /// Checks if the string is not null or empty.
        /// </summary>
        /// <param name="value">The string that needs to be checked</param>
        public static void StringCheck(string value)
        {
            if (value == null)
                throw new ArgumentNullException();
            else if (String.IsNullOrEmpty(value))
                throw new ArgumentException("String is empty.");
        }

        /// <summary>
        /// Checks if the id can be converted to an integer, and if it is postiive.
        /// </summary>
        /// <param name="value">The string that needs to be checked as an id.</param>
        public static void IdCheck(string value)
        {
            try
            {
                int id = Convert.ToInt32(value);
                if (id < 0)
                {
                    throw new ArgumentException("Id has to be positive.");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Id can not be converted to an integer.");
            }
        }
        
        /// <summary>
        /// Checks if the given string can be parsed to an AnswerState enum.
        /// </summary>
        /// <param name="answerState">The string that needs to be checked as an AnswerState</param>
        public static void AnswerStateCheck(string answerState)
        {
            try
            {
                AnswerState state = (AnswerState) Enum.Parse(typeof (AnswerState), answerState);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Cannot parse string to AnswerState enum.");
            }
        }

        /// <summary>
        /// Checks if the given string can be parsed to an ReviewState enum.
        /// </summary>
        /// <param name="reviewState">The string that needs to be checked as an ReviewState</param>
        public static void ReviewStateCheck(string reviewState)
        {
            try
            {
                ReviewState state = (ReviewState)Enum.Parse(typeof(ReviewState), reviewState);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Cannot parse string to ReviewState enum.");
            }
        }

        public static void SourceDefinitionExistsCheck(string SourceDefinitionName)
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
    }
}
