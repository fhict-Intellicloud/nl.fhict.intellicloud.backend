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
        
        [TestMethod]
        public void SendReplyTweetTest()
        {
            string reference = "";
            string answer = "No";
            string postId = "28289289298272762";

            //Reference can't be null or empty
            try
            {
                twitterManager.SendReplyTweet(answer, reference, postId);
                Assert.Fail();
            }
            catch (ArgumentException) { }

            reference = "@IntelliCloudQ";
            answer = "";

            //Answer can't be null or empty
            try
            {
                twitterManager.SendReplyTweet(answer, reference, postId);
                Assert.Fail();
            }
            catch (ArgumentException) { }

            answer = "Hello this answer is to long so it can't be send to twitter test test test test test test test test test test test test test test test test ";

            //Answer can't be more then 140 characters long
            try
            {
                twitterManager.SendReplyTweet(answer, reference, postId);
                Assert.Fail();
            }
            catch (ArgumentException) { }

            answer = "No";
            postId = "";

            //PostId can't be null or empty
            try
            {
                twitterManager.SendReplyTweet(answer, reference, postId);
                Assert.Fail();
            }
            catch (ArgumentException) { }
        }
    }
}
