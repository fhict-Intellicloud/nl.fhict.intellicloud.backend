using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace nl.fhict.IntelliCloud.Business.UnitTest
{
    [TestClass]
    public class WordStoreHelperTest
    {
        #region Fields

        #endregion Fields

        #region Tests

        #region question conversion tests
        /// <summary>
        /// Test if all punctuation is removed correctly.
        /// </summary>
        [TestMethod]
        public void convertQuestionTest_noPunctuation()
        {
            string question = "Jui*ste text ,./<>?;':\"[]{}\\|`~!@#$%^&*()_-+= die ov(er mo&et blijven.";

            IList<string> result = WordStoreHelper.ConvertQuestion(question);

            Assert.IsTrue(result.Except(new List<string>() { "Juiste", "text", "die", "over", "moet", "blijven" }).Count() == 0);
        }

        /// <summary>
        /// Test if the punctuation used within words is not removed.
        /// </summary>
        [TestMethod]
        public void convertQuestionTest_acceptedPunctuation()
        {

            string question = "midden-amerika andré hbo'er crèche û";

            IList<string> result = WordStoreHelper.ConvertQuestion(question);

            // Assert.IsTrue(result.Except(new List<string>() { "midden-amerika", "andré", "hbo'er", "crèche", "û" }).Count() == 0);
        }
        #endregion

        #region word resolving test

        /// <summary>
        /// Test if dutch words are correctly resolved.
        /// </summary>
        [TestMethod]
        public void ResolveWordsTest_Dutch_Noun()
        {
            string word = "virus";
            IList<Word> resolved = WordStoreHelper.ResolveWords(word);

            try
            {
                resolved.Where(x => x.Value == "virus" && x.Type == WordType.Noun && x.Language == Language.Dutch).Single();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Test if dutch words are correctly resolved.
        /// </summary>
        [TestMethod]
        public void ResolveWordsTest_Dutch_Verb()
        {
            string word = "formatteer";
            IList<Word> resolved = WordStoreHelper.ResolveWords(word);

            try
            {
                resolved.Where(x => x.Value == "formateren" && x.Type == WordType.Verb && x.Language == Language.Dutch).Single();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Test if english words are correctly resolved.
        /// </summary>
        [TestMethod]
        public void ResolveWordsTest_English_Noun()
        {
            string word = "virus";
            IList<Word> resolved = WordStoreHelper.ResolveWords(word);

            try
            {
                resolved.Where(x => x.Value == "virus" && x.Type == WordType.Noun && x.Language == Language.English).Single();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Test if dutch words are correctly resolved.
        /// </summary>
        [TestMethod]
        public void ResolveWordsTest_English_Verb()
        {
            string word = "format";
            IList<Word> resolved = WordStoreHelper.ResolveWords(word);

            try
            {
                resolved.Where(x => x.Value == "to format" && x.Type == WordType.Verb && x.Language == Language.English).Single();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Test that invalid words get returned with Unknown language and unknown wordType.
        /// </summary>
        [TestMethod]
        public void ResolveWordTest_Invalid()
        {
            string word = "formaturen";
            IList<Word> resolved = WordStoreHelper.ResolveWords(word);
            Assert.IsTrue(resolved[0].Language.Equals(Language.Unknown) && resolved[0].Type.Equals(WordType.Unknown));
        }

        #endregion

        #region find keywords tests

        /// <summary>
        /// Test if the keywords formateren, virus and computer are found in the following Question:
        /// "hoe formatteer ik mijn computer als ik een virus heb"
        /// </summary>
        [TestMethod]
        public void FindMostLikelyKeywordsTest_Dutch()
        {
            string question = "hoe formatteer ik mijn computer als ik een virus heb";

           var keywords =  WordStoreHelper.FindMostLikelyKeywords(question, Language.Dutch);

            try
            {
                keywords.Where(x => x.Value == "formateren" && x.Type == WordType.Verb && x.Language == Language.Dutch).Single();
                keywords.Where(x => x.Value == "virus" && x.Type == WordType.Noun && x.Language == Language.Dutch).Single();
                keywords.Where(x => x.Value == "computer" && x.Type == WordType.Noun && x.Language == Language.Dutch).Single();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Test if the keywords to format, virus and computer are found in the following Question:
        /// "How do I format my computer when i have a virus"
        /// </summary>
        [TestMethod]
        public void FindMostLikelyKeywordsTest_English()
        {
            string question = "How do I format my computer when i have a virus";

            IList<Word> keywords = WordStoreHelper.FindMostLikelyKeywords(
                WordStoreHelper.ResolveWords(question), 
                Language.English);

            try
            {
                keywords.Where(x => x.Value == "to format" && x.Type == WordType.Verb && x.Language == Language.English).Single();
                keywords.Where(x => x.Value == "virus" && x.Type == WordType.Noun && x.Language == Language.English).Single();
                keywords.Where(x => x.Value == "computer" && x.Type == WordType.Noun && x.Language == Language.English).Single();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
        #endregion

        #endregion Tests

        #region Test helpers


        /// <summary>
        /// Test helper function to compare two given words and return the result.
        /// </summary>
        /// <param name="w1">First word in the comparison</param>
        /// <param name="w2">Second word in the comparison</param>
        /// <returns>True us returned when two words are equal, else false is returned.</returns>
        private bool isEquals(Word w1, Word w2)
        {
            return w1.Language.Equals(w2.Language) && w1.Type.Equals(w2.Type) && w1.Value.Equals(w2.Type);
        }
        #endregion
    }
}
