using IntelliCloud.WordStore.Common.Word;
using IntelliCloud.WordStore.Data.Database.Factory;
using IntelliCloud.WordStore.Data.Database.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IntelliCloud.WordStore.Service.IntegrationTest
{
    [TestClass]
    public class WordServiceTest
    {
        #region Fields

        private IWordService service;

        #endregion Fields

        #region Methods

        [TestInitialize]
        public void Initialize()
        {
            this.service = new WordService();
        }

        #region Tests

        [TestMethod]
        [TestCategory("IntelliCloud.Service.IntegrationTest")]
        public void ResolveWords_Valid()
        {
            var words = this.service.ResolveWord("show");

            Assert.AreEqual(3, words.Count());
            Assert.IsNotNull(words.SingleOrDefault(
                    word1 => word1.Value == "show"
                    && word1.Type == WordType.Noun
                    && word1.Language == Language.English));
            Assert.IsNotNull(words.SingleOrDefault(
                    word2 => word2.Value == "to show"
                    && word2.Type == WordType.Verb
                    && word2.Language == Language.English));
            Assert.IsNotNull(words.SingleOrDefault(
                    word3 => word3.Value == "show"
                    && word3.Type == WordType.Noun
                    && word3.Language == Language.Dutch));
        }

        [TestMethod]
        [TestCategory("IntelliCloud.Service.IntegrationTest")]
        public void ResolveWords_Unknown()
        {
            var words = this.service.ResolveWord("AnUnknownWord");

            Assert.AreEqual(1, words.Count());
            Assert.IsNotNull(words.SingleOrDefault(
                    word1 => word1.Value == "AnUnknownWord"
                    && word1.Type == WordType.Unknown
                    && word1.Language == Language.Unknown));
        }

        #endregion Tests

        #endregion Methods
    }
}
