using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nl.fhict.IntelliCloud.Business.UnitTest
{
    [TestClass]
    public class ValidationTest
    {
        #region Fields


        #endregion Fields

        #region Methods

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
                Validation.StringCheck(String.Empty);
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
                Validation.StringCheck(null);
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
                Validation.IdCheck("UnparseableValue");
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
                Validation.IdCheck("-1");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Id has to be positive.", e.Message);
            }
        }

        #endregion IdCheck

        #endregion Tests

        #endregion Methods
    }
}
