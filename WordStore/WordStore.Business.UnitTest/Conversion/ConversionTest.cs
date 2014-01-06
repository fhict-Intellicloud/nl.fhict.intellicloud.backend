using IntelliCloud.WordStore.Business.Conversion;
using IntelliCloud.WordStore.Common.Word;
using IntelliCloud.WordStore.Data.Database.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordObject = IntelliCloud.WordStore.Common.Word.Word;

namespace IntelliCloud.WordStore.Business.UnitTest.Conversion
{
    /// <summary>
    /// A test class for <see cref="Conversion"/>.
    /// </summary>
    [TestClass]
    public class ConversionTest
    {
        /// <summary>
        /// The instance of <see cref="Conversion"/> that is being tested.
        /// </summary>
        private IConversion conversion;

        [TestInitialize]
        public void Initialize()
        {
            this.conversion = new Business.Conversion.Conversion();
        }

        [TestMethod]
        public void ToWord()
        {
            WordEntity entity = new WordEntity()
            {
                Value = "value",
                Type = WordType.Article,
                Language = Language.Dutch,
            };

            var word = this.conversion.ToWord(entity);
            Assert.AreEqual(entity.Value, word.Value);
            Assert.AreEqual(entity.Type, word.Type);
            Assert.AreEqual(entity.Language, word.Language);
        }

        [TestMethod]
        public void ToWordType()
        {
            Assert.AreEqual(WordType.Adjective, this.conversion.ToWordType("bijvoeglijk naamwoord"));
            Assert.AreEqual(WordType.Verb, this.conversion.ToWordType("werkwoord"));
            Assert.AreEqual(WordType.Noun, this.conversion.ToWordType("zelfstandig naamwoord"));
            Assert.AreEqual(WordType.Interjection, this.conversion.ToWordType("tussenwoord"));
            Assert.AreEqual(WordType.Adverb, this.conversion.ToWordType("bijwoord"));
            Assert.AreEqual(WordType.Pronoun, this.conversion.ToWordType("voornaamwoord"));
            Assert.AreEqual(WordType.Article, this.conversion.ToWordType("lidwoord"));
            Assert.AreEqual(WordType.Unknown, this.conversion.ToWordType("any other"));
        }

        [TestMethod]
        public void ToLanguage()
        {
            Assert.AreEqual(Language.Dutch, this.conversion.ToLanguage("nl"));
            Assert.AreEqual(Language.English, this.conversion.ToLanguage("uk"));
            Assert.AreEqual(Language.English, this.conversion.ToLanguage("us"));
            Assert.AreEqual(Language.Unknown, this.conversion.ToLanguage("any other"));
        }

        [TestMethod]
        public void ToKeywordEntity()
        {
            string keyword = "keyword";
            var words = Enumerable.Repeat(new WordObject("value", WordType.Adverb, Language.English), 5);

            var entity = this.conversion.ToKeywordEntity(keyword, words);
            Assert.AreEqual(keyword, entity.Value);
            Assert.AreEqual(words.Count(), entity.Words.Count());
            entity.Words.ToList().ForEach(
                word =>
            {
                Assert.AreEqual("value", word.Value);
                Assert.AreEqual(WordType.Adverb, word.Type);
                Assert.AreEqual(Language.English, word.Language);
            });
        }
    }
}
