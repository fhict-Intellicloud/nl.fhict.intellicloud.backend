using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nl.fhict.IntelliCloud.Business.Manager;
using nl.fhict.IntelliCloud.Data.WordStoreService;

namespace nl.fhict.IntelliCloud.Service.IntegrationTest
{

    /// <summary>
    /// This class inhibits integration tests for the calls to the wordservice. 
    /// This service is needed for the keyword algorithm.
    /// </summary>
    [TestClass]
    public class KeywordAlgorithmTest
    {
        #region Fields

        /// <summary>
        /// Instance of the question manager class.
        /// </summary>
        private QuestionManager manager;

        #endregion Fields

        #region Methods

        /// <summary>
        /// A method that is called before each test is run. This method is used to set up a fresh state for
        /// each test by for instance creating new service objects.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.manager = new QuestionManager();
        }

        /// <summary>
        /// A method that is called after each test that is ran. This method is used to, for instance, dispose
        /// any objects that require disposing.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
        }

        #region Tests

        #region Word resolving tests

        /// <summary>
        /// Test if dutch words are correctly resolved.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void ResolveWordsTest_Dutch_Noun()
        {
            string word = "virus";
            IList<Word> resolved = this.manager.ResolveWords(word);

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
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void ResolveWordsTest_Dutch_Verb()
        {
            string word = "formatteer";
            IList<Word> resolved = this.manager.ResolveWords(word);

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
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void ResolveWordsTest_English_Noun()
        {
            string word = "virus";
            IList<Word> resolved = this.manager.ResolveWords(word);

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
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void ResolveWordsTest_English_Verb()
        {
            string word = "format";
            IList<Word> resolved = this.manager.ResolveWords(word);

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
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void ResolveWordTest_Invalid()
        {
            string word = "formaturen";
            IList<Word> resolved = this.manager.ResolveWords(word);
            Assert.IsTrue(resolved[0].Language.Equals(Language.Unknown) && resolved[0].Type.Equals(WordType.Unknown));
        }

        #endregion Word resolving tests

        #region Find keywords tests

        /// <summary>
        /// Test if the keywords formateren, virus and computer are found in the following Question:
        /// "hoe formatteer ik mijn computer als ik een virus heb"
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void FindMostLikelyKeywordsTest_Dutch()
        {
            string question = "hoe formatteer ik mijn computer als ik een virus heb";
            IList<Word> resolvedWords = this.manager.ResolveWords(question);

            IList<Word> keywords = this.manager.FindMostLikelyKeywords(
                this.manager.ResolveWords(question),
                Language.Dutch);

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
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void FindMostLikelyKeywordsTest_English()
        {
            string question = "How do I format my computer when i have a virus";

            IList<Word> keywords = this.manager.FindMostLikelyKeywords(
                this.manager.ResolveWords(question),
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

        #region Get language tests

        /// <summary>
        /// Test if the langauge can succesfully be found from the sentence:
        /// "Ik heb een virus op mijn computer hoe kan ik deze vinden en verwijderen."
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetLanguage_Dutch()
        {
            Language foundLanguge = this.manager.GetLanguage(
                    this.manager.ResolveWords("Ik heb een virus op mijn computer hoe kan ik deze vinden en verwijderen."));

            Assert.IsTrue(foundLanguge.Equals(Language.Dutch));
        }

        /// <summary>
        /// Test if the langauge can succesfully be found from the sentence:
        /// "I Found a virus on my desktop, how can i delete this."
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetLanguage_English()
        {
            Language foundLanguge = this.manager.GetLanguage(
                    this.manager.ResolveWords("I Found a virus on my desktop, how can i delete this."));

            Assert.IsTrue(foundLanguge.Equals(Language.English));
        }

        #endregion Get language tests

        #endregion Tests

        #endregion Methods
    }
}
