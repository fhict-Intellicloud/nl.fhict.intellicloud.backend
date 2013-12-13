using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nl.fhict.IntelliCloud.Service.IntegrationTest.Manager
{
    /// <summary>
    /// This class inhibits the integration tests for the AnswerService.
    /// </summary>
    [TestClass]
    public class AnswerServiceTest
    {
        #region Fields

        /// <summary>
        /// An instance of the service AnswerService that is being tested by this class.
        /// </summary>
        private IAnswerService service;

        #endregion Fields

        #region Methods

        /// <summary>
        /// A method that is called before each test is run. This method is used to set up a fresh state for
        /// each test by for instance creating new service objects.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.service = new ReviewService();
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

       

        #endregion Tests

        #endregion Methods
    }
}
