using Microsoft.VisualStudio.TestTools.UnitTesting;
using nl.fhict.IntelliCloud.Business.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nl.fhict.IntelliCloud.Business.UnitTest.Manager
{
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

        // Tests here

        #endregion Tests

        #endregion Methods
    }
}
