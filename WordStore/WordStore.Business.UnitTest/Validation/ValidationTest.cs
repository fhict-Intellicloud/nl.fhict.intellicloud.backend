using IntelliCloud.WordStore.Business.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliCloud.WordStore.Business.UnitTest.Validation
{
    /// <summary>
    /// A test class for <see cref="Validation"/>.
    /// </summary>
    [TestClass]
    public class ValidationTest
    {
        /// <summary>
        /// The instance of <see cref="Validation"/> that is being tested.
        /// </summary>
        private IValidation validation;

        /// <summary>
        /// An initialization method that is ran before each test to set up a fresh state.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.validation = new Business.Validation.Validation();
        }

        /// <summary>
        /// A test for method <see cref="ValidateWord"/>. This test verifies that an exception is thrown
        /// when <c>null</c> is passed as an argument.
        /// </summary>
        [TestMethod]
        public void ValidateWord_Null()
        {
            try
            {
                this.validation.ValidateWord(null);
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.IsTrue(e.Message.Contains("The word cannot be equal to null"));
            }
        }

        /// <summary>
        /// A test for method <see cref="ValidateWord"/>. This test verifies that an exception is thrown
        /// when <c>string.Empty</c> is passed as an argument.
        /// </summary>
        [TestMethod]
        public void ValidateWord_Empty()
        {
            try
            {
                this.validation.ValidateWord(string.Empty);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.IsTrue(e.Message.Contains("The word cannot be an empty string or only contain whitespace characters."));
            }
        }

        /// <summary>
        /// A test for method <see cref="ValidateWord"/>. This test verifies that an exception is thrown
        /// when a string only containing whitespace is passed as an argument.
        /// </summary>
        [TestMethod]
        public void ValidateWord_Whitespace()
        {
            try
            {
                this.validation.ValidateWord("  ");
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.IsTrue(e.Message.Contains("The word cannot be an empty string or only contain whitespace characters."));
            }
        }

        /// <summary>
        /// A test for method <see cref="ValidateWord"/>. This test verifies that an exception is thrown
        /// when a string containing multiple words is passed as an argument.
        /// </summary>
        [TestMethod]
        public void ValidateWord_MultipleWords()
        {
            try
            {
                this.validation.ValidateWord("word1 word2");
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.IsTrue(e.Message.Contains("The word must consist of a single word without spaces."));
            }
        }

        /// <summary>
        /// A test for method <see cref="ValidateWord"/>. This test verifies that an exception is thrown
        /// when a string containing a single word with whitespace is passed as an argument.
        /// </summary>
        [TestMethod]
        public void ValidateWord_SingleWordWhitespace()
        {
            try
            {
                this.validation.ValidateWord("word1 ");
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.IsTrue(e.Message.Contains("The word must consist of a single word without spaces."));
            }
        }

        /// <summary>
        /// A test for method <see cref="ValidateWord"/>. This test verifies that no exception is thrown
        /// when a string containing a single valid word.
        /// </summary>
        [TestMethod]
        public void ValidateWord_SingleWordValid()
        {
            this.validation.ValidateWord("word");
        }
    }
}
