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

        /// <summary>
        /// Method that gets called each time a test is run.
        /// </summary>
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
        [ExpectedException(typeof(ArgumentException))]
        public void IdCheck_NegativeInt()
        {
            validation.IdCheck(-1);
        }

        /// <summary>
        /// Test if the correct exception is thrown when passing a value that cannot be converted to an integer
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IdCheck_Unparseable()
        {
            validation.IdCheck("UnparseableValue");
        }

        /// <summary>
        /// Test if the correct exception is thrown when passing a negative value
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IdCheck_Negative()
        {
            validation.IdCheck("-1");
        }

        /// <summary>
        /// Test if the correct exception is thrown when passing a null value
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IdCheck_Null()
        {
            validation.IdCheck(null);
        }

        #endregion IdCheck

        #region TweetLengthCheck

        /// <summary>
        /// Test if the correct exception is thrown when passing a tweet that's too long
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TweetLengthCheck_Length()
        {
            validation.TweetLengthCheck("Hello this answer is too long so it can't be send to twitter test test test test test test test test test test test test test test test test!!");
        }

        /// <summary>
        /// Test if the correct exception is thrown when passing a tweet empty
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TweetLengthCheck_Empty()
        {
                validation.TweetLengthCheck("");
        }

        /// <summary>
        /// Test if the correct exception is thrown when passing a tweet empty
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TweetLengthCheck_Null()
        {
            validation.TweetLengthCheck(null);
        }

        #endregion TweetLengthCheck

        #endregion Tests

        #endregion Methods
    }
}
