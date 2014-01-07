using System;
using System.Reflection;
using System.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nl.fhict.IntelliCloud.Business.Plugins;
using nl.fhict.IntelliCloud.Business.UnitTest.Properties;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;

namespace nl.fhict.IntelliCloud.Business.UnitTest
{
    /// <summary>
    /// A testclass to test twittersendplugin code
    /// </summary>
    [TestClass]
    public class TwitterSendAnswerPluginTest
    {
        #region Fields

        TwitterSendAnswerPlugin twitterSendAnswerPlugin;

        #endregion Fields

        #region Methods

        [TestInitialize]
        public void Initialize()
        {
            twitterSendAnswerPlugin = new TwitterSendAnswerPlugin(Resources.ResourceManager);
        }

        #region Tests

        /// <summary>
        /// Tests if the length answer string is checked, can't be larger than 140 charactes.
        /// </summary>
        [TestMethod]
        public void SendLongReplyTweetAnswerTest()
        {
            QuestionEntity question = new QuestionEntity();
            question.Source = new QuestionSourceEntity();
            question.Source.Source = new SourceEntity();
            question.Source.PostId = "1";
            question.Source.Source.Value = "@IntelliCloudQ";

            AnswerEntity answer = new AnswerEntity();
            answer.Content = "Hello this answer is too long so it can't be send to twitter test test test test test test test test test test test test test!!";

            //Answer can't be more then 140 characters long
            try
            {
                twitterSendAnswerPlugin.SendAnswer(question, answer);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests if the answer string is checked, can't be empty
        /// </summary>
        [TestMethod]
        public void SendEmptyReplyTweetAnswerTest()
        {
            QuestionEntity question = new QuestionEntity();
            question.Source = new QuestionSourceEntity();
            question.Source.Source = new SourceEntity();
            question.Source.PostId = "1";
            question.Source.Source.Value = "";

            AnswerEntity answer = new AnswerEntity();
            answer.Content = "";

            //Answer can't be empty
            try
            {
                twitterSendAnswerPlugin.SendAnswer(question, answer);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests if the answer string is checked, can't be null
        /// </summary>
        [TestMethod]
        public void SendNullEmptyReplyTweetAnswerTest()
        {
            QuestionEntity question = new QuestionEntity();
            question.Source = new QuestionSourceEntity();
            question.Source.Source = new SourceEntity();
            question.Source.PostId = "1";
            question.Source.Source.Value = null;

            AnswerEntity answer = new AnswerEntity();
            answer.Content = null;

            //Answer can't be empty
            try
            {
                twitterSendAnswerPlugin.SendAnswer(question, answer);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests if the postId string is checked, can't be empty
        /// </summary>
        [TestMethod]
        public void SendEmptyReplyTweetPostIdTest()
        {
            QuestionEntity question = new QuestionEntity();
            question.Source = new QuestionSourceEntity();
            question.Source.Source = new SourceEntity();
            question.Source.PostId = "";
            question.Source.Source.Value = "@IntelliCloudQ";

            AnswerEntity answer = new AnswerEntity();
            answer.Content = "This is a valid answer";

            try
            {
                twitterSendAnswerPlugin.SendAnswer(question, answer);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests if the postId string is checked, can't be null
        /// </summary>
        [TestMethod]
        public void SendNullEmptyReplyTweetPostIdTest()
        {
            QuestionEntity question = new QuestionEntity();
            question.Source = new QuestionSourceEntity();
            question.Source.Source = new SourceEntity();
            question.Source.PostId = null;
            question.Source.Source.Value = "@IntelliCloudQ";

            AnswerEntity answer = new AnswerEntity();
            answer.Content = "This is a valid answer";

            try
            {
                twitterSendAnswerPlugin.SendAnswer(question, answer);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests if the postId string is checked, can't be empty
        /// </summary>
        [TestMethod]
        public void SendEmptyReplyReceivedPostIdTest()
        {
            QuestionEntity question = new QuestionEntity();
            question.Source = new QuestionSourceEntity();
            question.Source.Source = new SourceEntity();
            question.Source.PostId = "";
            question.Source.Source.Value = "@IntelliCloudQ";

            try
            {
                twitterSendAnswerPlugin.SendQuestionReceived(question);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests if the postId string is checked, can't be null
        /// </summary>
        [TestMethod]
        public void SendNullEmptyReplyReceivedPostIdTest()
        {
            QuestionEntity question = new QuestionEntity();
            question.Source = new QuestionSourceEntity();
            question.Source.Source = new SourceEntity();
            question.Source.PostId = null;
            question.Source.Source.Value = "@IntelliCloudQ";

            try
            {
                twitterSendAnswerPlugin.SendQuestionReceived(question);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        #endregion Tests

        #endregion Methods
    }
}
