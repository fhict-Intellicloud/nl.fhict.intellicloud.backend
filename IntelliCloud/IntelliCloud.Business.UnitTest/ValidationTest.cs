using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nl.fhict.IntelliCloud.Business.UnitTest
{
    /// <summary>
    /// In this unit test class all methods of the Validation class will be tested.
    /// </summary>
    [TestClass]
    public class ValidationTest
    {
        #region Fields

        /// <summary>
        /// Validation object with which its functionality is tested.
        /// </summary>
        private Validation validation;

        #endregion Fields

        #region Methods

        [TestInitialize]
        public void Initialize()
        {
            validation = new Validation();
        }

        #region Tests

        #region StringCheck

        /// <summary>
        /// Test if the correct exception is thrown when passing an empty string
        /// </summary>
        [TestMethod]
        public void StringCheck_Empty()
        {
            try
            {
                validation.StringCheck(String.Empty);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
                Assert.AreEqual("String is empty.", e.Message);
            }
        }

        /// <summary>
        /// Test if the correct exception is thrown when passing a null value
        /// </summary>
        [TestMethod]
        public void StringCheck_Null()
        {
            try
            {
                validation.StringCheck(null);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentNullException);
            }
        }

        #endregion StringCheck

        #region IdCheck

        /// <summary>
        /// Test if the correct exception is thrown when passing a value that cannot be converted to an integer
        /// </summary>
        [TestMethod]
        public void IdCheck_Unparseable()
        {
            try
            {
                validation.IdCheck("UnparseableValue");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Id can not be converted to an integer.", e.Message);
            }
        }

        /// <summary>
        /// Test if the correct exception is thrown when passing a negative value
        /// </summary>
        [TestMethod]
        public void IdCheck_Negative()
        {
            try
            {
                validation.IdCheck("-1");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Id has to be positive.", e.Message);
            }
        }

        #endregion IdCheck

        #region TweetLengthCheck

        /// <summary>
        /// Test if the correct exception is thrown when passing a tweet that's too long
        /// </summary>
        [TestMethod]
        public void TweetLengthCheck_Length()
        {
            try
            {
                validation.TweetLengthCheck("Hello this answer is too long so it can't be send to twitter test test test test test test test test test test test test test test test test!!");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Tweet can't be longer then 140 characters", e.Message);
            }
        }

        /// <summary>
        /// Test if the correct exception is thrown when passing a tweet empty
        /// </summary>
        [TestMethod]
        public void TweetLengthCheck_Empty()
        {
            try
            {
                validation.TweetLengthCheck("");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Tweet is empty", e.Message);
            }
        }

        #endregion TweetLengthCheck

        #endregion Tests

        #endregion Methods
    }
}
