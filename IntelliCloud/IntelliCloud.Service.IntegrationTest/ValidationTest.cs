using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nl.fhict.IntelliCloud.Business;

namespace nl.fhict.IntelliCloud.Service.IntegrationTest.Services
{

    /// <summary>
    /// Test class to validate the validation object.
    /// </summary>
    [TestClass]
    public class ValidationTest
    {
        #region Fields

        /// <summary>
        /// Validation object with which its functionality is tested.
        /// </summary>
        private IValidation validation;

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

        /// <summary>
        /// Test to validate if sourceDefinationExists function works.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckSourceDefinationExists()
        {
            this.validation.SourceDefinitionExistsCheck("UnexistingSource");
        }

        #endregion Tests

        #endregion Methods
    }
}
