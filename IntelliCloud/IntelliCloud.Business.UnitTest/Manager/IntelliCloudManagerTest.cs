using System.Runtime.Remoting.Contexts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using nl.fhict.IntelliCloud.Business.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;

namespace nl.fhict.IntelliCloud.Business.UnitTest.Manager
{
    /// <summary>
    /// In this unit test class all methods of the IntelliCloudManager will be tested.
    /// All these test will use mock data.
    /// </summary>
    [TestClass]
    public class IntelliCloudManagerTest
    {
        #region Fields

        private IntelliCloudManager manager;

        #endregion Fields

        #region Methods

        [TestInitialize]
        public void Initialize()
        {
            this.manager = new IntelliCloudManager();
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        #region Tests

        [TestMethod]
        public void UpdateReviewTest()
        {
            var mock = new Mock<IntelliCloudContext>();
            //mock.Setup(context => context.Reviews.Add(new ReviewEntity() {  }));
            //http://code.google.com/p/moq/wiki/QuickStart

        }

        #endregion Tests

        #endregion Methods
    }
}
