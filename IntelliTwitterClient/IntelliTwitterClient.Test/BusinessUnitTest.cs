using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntelliTwitterClient.Business.Managers;

namespace IntelliTwitterClient.Test
{
    /// <summary>
    /// A testclass to test the tweetmanager code
    /// </summary>
    [TestClass]
    public class BusinessUnitTest
    {
        //Private object to test private methods of the TwitterManager
        PrivateObject privateTwitterManager;

        [TestInitialize]
        public void InitializeTest()
        {
            var twitterManager = new TwitterManager(new System.Diagnostics.EventLog());
            privateTwitterManager = new PrivateObject(twitterManager);
        }

        /// <summary>
        /// Tests if the reference string is checked, can't be null or empty
        /// </summary>
        [TestMethod]
        public void CreateQuestionReferenceTest()
        {
            var reference = "";
            var question = "How does it feel?";
            var postId = "28289289298272762";
            
            try
            {
                privateTwitterManager.Invoke("CreateQuestion", reference, question, postId);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
                Assert.AreEqual("String is empty.", e.Message);
            }
        }

        /// <summary>
        /// Tests if the question string is checked, can't be null or empty
        /// </summary>
        [TestMethod]
        public void CreateQuestionQuestionTest()
        {
            var reference = "@IntelliCloudQ";
            var question = "";
            var postId = "28289289298272762";

            //Question can't be null or empty
            try
            {
                privateTwitterManager.Invoke("CreateQuestion", reference, question, postId);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
                Assert.AreEqual("String is empty.", e.Message);
            }
        }

        /// <summary>
        /// Tests if the postId string is checked, can't be null or empty
        /// </summary>
        [TestMethod]
        public void CreateQuestionPostIdTest()
        {
            var reference = "@IntelliCloudQ";
            var question = "How does it feel?";
            var postId = "";

            //PostId can't be null or empty
            try
            {
                privateTwitterManager.Invoke("CreateQuestion", reference, question, postId);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
                Assert.AreEqual("String is empty.", e.Message);
            }
        }
    }
}
