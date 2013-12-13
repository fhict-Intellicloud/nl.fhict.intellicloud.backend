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
            twitterSendAnswerPlugin = new TwitterSendAnswerPlugin();
        }

        #region Tests

        [TestMethod]
        public void SendReplyTweetTest()
        {
            QuestionSource questionSource = new QuestionSource();
            questionSource.PostId = "1";

            Source source = new Source();
            source.Value = "@IntelliCloudQ";

            questionSource.Source = source;

            Question question = new Question();
            question.Source = questionSource;

            Answer answer = new Answer();
            answer.Content = "Hello this answer is too long so it can't be send to twitter test test test test test test test test test test test test test!!";
            
            //Answer can't be more then 140 characters long
            try
            {
                twitterSendAnswerPlugin.SendAnswer(question, answer);
                Assert.Fail();
            }
            catch (ArgumentException) { }

            question.Source.PostId = "";
            answer.Content = "This is an valid answer";

            //PostId can't be null
            try
            {
                twitterSendAnswerPlugin.SendAnswer(question, answer);
                Assert.Fail();
            }
            catch (ArgumentException) { }
        }

        #endregion Tests

        #endregion Methods
    }
}
