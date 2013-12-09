using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using nl.fhict.IntelliCloud.Business.Manager;
using nl.fhict.IntelliCloud.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Business.WordService;

namespace nl.fhict.IntelliCloud.Business.UnitTest.Manager
{
    /// <summary>
    /// In this unit test class all methods of the QuestionManager will be tested.
    /// All these test will use mock classes.
    /// </summary>
    [TestClass]
    public class QuestionManagerTest
    {
        #region Fields

        private QuestionManager manager;
        private Mock<IValidation> validation;

        #endregion Fields

        #region Methods

        [TestInitialize]
        public void Initialize()
        {
            validation = new Mock<IValidation>();
            this.manager = new QuestionManager(new IntelliCloudContext(), validation.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        #region Tests

        #region question manager tests
        /// <summary>
        /// In this test we check if the questionId and emplooyeeId is being validated in the UpdateQuestion method.
        /// </summary>
        [TestMethod]
        public void UpdateQuestionTest()
        {
            int questionId = 1;
            int employeeId = 1;

            try
            {
                manager.UpdateQuestion(questionId, employeeId);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(questionId), Times.Once());
            validation.Verify(v => v.IdCheck(employeeId), Times.Once());
        }

        /// <summary>
        /// In this test we check if the source, reference, question, title, postId and isPrivate is being validated in the CreateQuestion method.
        /// </summary>
        [TestMethod]
        public void CreateQuestionTest()
        {
            string source = "Mail";
            string reference = "Test@Gmail.coom";
            string question = "This is my question";
            string title = "This is my title";
            string postId = "";
            bool isPrivate = true;

            try
            {
                manager.CreateQuestion(source, reference, question, title, postId, isPrivate);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.StringCheck(source), Times.Once());
            validation.Verify(v => v.StringCheck(reference), Times.Once());
            validation.Verify(v => v.StringCheck(question), Times.Once());
            validation.Verify(v => v.StringCheck(title), Times.Once());
            validation.Verify(v => v.StringCheck(postId), Times.Once());
            // TODO: boolean check
        }

        /// <summary>
        /// In this test we check if the qustionId is being validated in the GetQuestion method.
        /// </summary>
        [TestMethod]
        public void GetQuestionTest()
        {
            int questionId = 1;

            try
            {
                manager.GetQuestion(questionId);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(questionId), Times.Once());
        }

        /// <summary>
        /// In this test we check if the employeeId is being validated in the GetQuestions method.
        /// </summary>
        [TestMethod]
        public void GetQuestionsTest()
        {
            int employeeId = 1;

            try
            {
                manager.GetQuestions(employeeId);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(employeeId), Times.Once());
        }

        #endregion

        #region Keyword algorithm tests
        #region Question conversion tests
        /// <summary>
        /// Test if all punctuation is removed correctly.
        /// </summary>
        [TestMethod]
        public void convertQuestionTest_noPunctuation()
        {
            string question = "Jui*ste text ,./<>?;':\"[]{}\\|`~!@#$%^&*()_-+= die ov(er mo&et blijven.";
            IList<string> result = this.manager.ConvertQuestion(question);

            Assert.IsTrue(result.Except(new List<string>() { "Juiste", "text", "die", "over", "moet", "blijven" }).Count() == 0);
        }

        /// <summary>
        /// Test if the punctuation used within words is not removed.
        /// </summary>
        [TestMethod]
        public void convertQuestionTest_acceptedPunctuation()
        {
            string question = "midden-amerika andré hbo'er crèche û";
            IList<string> result = this.manager.ConvertQuestion(question);

            Assert.IsTrue(result.Except(new List<string>() { "midden-amerika", "andré", "hbo'er", "crèche", "û" }).Count() == 0);
        }
        #endregion  Question conversion tests

        #region Word resolving tests

        /// <summary>
        /// Test if dutch words are correctly resolved.
        /// </summary>
        [TestMethod]
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
        public void GetLanguage_English()
        {
            Language foundLanguge = this.manager.GetLanguage(
                    this.manager.ResolveWords("I Found a virus on my desktop, how can i delete this."));

            Assert.IsTrue(foundLanguge.Equals(Language.English));
        }

        #endregion Get language tests

        #endregion Keyword algorithm tests

        #endregion Tests

        #endregion Methods
    }
}
