using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntelliTwitterClient.Business.Managers;

namespace IntelliTwitterClient.Test
{
    [TestClass]
    public class BusinessUnitTest
    {
        TwitterManager twitterManager;

        [TestInitialize]
        public void InitializeTest()
        {
            twitterManager = new TwitterManager(new System.Diagnostics.EventLog());
        }

        [TestMethod]
        public void CreateQuestionTest()
        {
            var privateTwitterManager = new PrivateObject(twitterManager);

            string reference = "";
            string question = "How does it feel?";
            string postId = "28289289298272762";

            //Refererence can't be null or empty
            try
            {
                privateTwitterManager.Invoke("CreateQuestion", reference, question, postId);
                Assert.Fail();
            }
            catch (ArgumentException) { }

            reference = "@IntelliCloudQ";
            question = "";

            //Question can't be null or empty
            try
            {
                privateTwitterManager.Invoke("CreateQuestion", reference, question, postId);
                Assert.Fail();
            }
            catch (ArgumentException) { }

            question = "How does it feel?";
            postId = "";

            //PostId can't be null or empty
            try
            {
                privateTwitterManager.Invoke("CreateQuestion", reference, question, postId);
                Assert.Fail();
            }
            catch (ArgumentException) { }

        }
    }
}
