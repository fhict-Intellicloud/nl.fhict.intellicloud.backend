using Microsoft.VisualStudio.TestTools.UnitTesting;
using nl.fhict.IntelliCloud.Business.Plugins;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nl.fhict.IntelliCloud.Business.UnitTest
{
    /// <summary>
    /// A testclass to test twittersendplugin code
    /// </summary>
    [TestClass]
    public class TwitterSendAnswerPluginTest
    {
    ////    #region Fields

    ////    TwitterSendAnswerPlugin twitterSendAnswerPlugin;
    ////    Question question;
    ////    Answer answer;

    ////    #endregion Fields

    ////    #region Methods

    ////    [TestInitialize]
    ////    public void Initialize()
    ////    {
    ////        twitterSendAnswerPlugin = new TwitterSendAnswerPlugin();
    ////        QuestionSource questionSource = new QuestionSource();
    ////        Source source = new Source();
    ////        questionSource.Source = source;
    ////        question = new Question();
    ////        question.Source = questionSource;
    ////        answer = new Answer();
    ////    }

    ////    #region Tests

    ////    /// <summary>
    ////    /// Tests if the answer string is checked, can't be null or empty
    ////    /// </summary>
    ////    [TestMethod]
    ////    public void SendReplyTweetAnswerTest()
    ////    {
    ////        question.Source.PostId = "1";
    ////        question.Source.Value = "@IntelliCloudQ";
            
    ////        answer.Content = "Hello this answer is too long so it can't be send to twitter test test test test test test test test test test test test test!!";

    ////        //Answer can't be more then 140 characters long
    ////        try
    ////        {
    ////            twitterSendAnswerPlugin.SendAnswer(question, answer);
    ////            Assert.Fail();
    ////        }
    ////        catch (Exception e)
    ////        {
    ////            Assert.IsTrue(e is ArgumentException);
    ////            Assert.AreEqual("Tweet can't be longer then 140 characters", e.Message);
    ////        }

    ////        question.Source.Source.Value = "";
    ////        answer.Content = "";

    ////        //Answer can't be empty
    ////        try
    ////        {
    ////            twitterSendAnswerPlugin.SendAnswer(question, answer);
    ////            Assert.Fail();
    ////        }
    ////        catch (Exception e)
    ////        {
    ////            Assert.IsTrue(e is ArgumentException);
    ////            Assert.AreEqual("Tweet is empty", e.Message);
    ////        }
    ////    }

    ////    /// <summary>
    ////    /// Tests if the postId string is checked, can't be null or empty
    ////    /// </summary>
    ////    [TestMethod]
    ////    public void SendReplyTweetPostIdTest()
    ////    {
    ////        question.Source.PostId = "";
    ////        question.Source.Source.Value = "@IntelliCloudQ";
           
    ////        answer.Content = "This is a valid answer";

    ////        try
    ////        {
    ////            twitterSendAnswerPlugin.SendAnswer(question, answer);
    ////            Assert.Fail();
    ////        }
    ////        catch (Exception e)
    ////        {
    ////            Assert.IsTrue(e is ArgumentException);
    ////            Assert.AreEqual("String is empty.", e.Message);
    ////        }
    ////    }

    ////    #endregion Tests

    ////    #endregion Methods
    }
}
