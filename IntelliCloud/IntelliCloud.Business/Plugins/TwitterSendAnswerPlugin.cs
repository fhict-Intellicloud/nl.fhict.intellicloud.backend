using System.Configuration;
using System.Resources;
using LinqToTwitter;
using nl.fhict.IntelliCloud.Business.Plugins.Loader;
using nl.fhict.IntelliCloud.Business.Properties;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;

namespace nl.fhict.IntelliCloud.Business.Plugins
{
    /// <summary>
    /// A plugin to handle outgoing tweets
    /// </summary>
    public class TwitterSendAnswerPlugin : ISendAnswerPlugin
    {
        private IValidation validation;
        private ResourceManager resourceManager;

        /// <summary>
        /// The pin authorizer that is needed since twitter API 1.1.
        /// </summary>
        private readonly static PinAuthorizer PinAuthorizedUser = new PinAuthorizer
        {
            Credentials = new InMemoryCredentials
            {
                ConsumerKey = ConfigurationManager.AppSettings["IntelliCloud.Twitter.ConsumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["IntelliCloud.Twitter.ConsumerSecret"],
                OAuthToken = ConfigurationManager.AppSettings["IntelliCloud.Twitter.OAuthToken"],
                AccessToken = ConfigurationManager.AppSettings["IntelliCloud.Twitter.AccessToken"]
            }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterSendAnswerPlugin"/> class.
        /// </summary>
        public TwitterSendAnswerPlugin(ResourceManager resourceManager)
        {
            validation = new Validation();
            this.resourceManager = resourceManager;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterSendAnswerPlugin"/> class.
        /// </summary>
        public TwitterSendAnswerPlugin()
        {
            validation = new Validation();
            this.resourceManager = Resources.ResourceManager;
        }
        
        /// <summary>
        /// Sends a confirmation the the asker of the question
        /// </summary>
        /// <param name="question">the question asked by a user</param>
        public void SendQuestionReceived(QuestionEntity question)
        {
            var reference = question.Source.Source.Value;
            var postId = question.Source.PostId;

            string tweetBody = resourceManager.GetString(question.LanguageDefinition.ResourceName + "_TWITTER_AUTO_RESPONSE");

            var status = reference + " " + tweetBody;

            validation.StringCheck(postId);
            validation.TweetLengthCheck(status);

            using (TwitterContext twitterCtx = new TwitterContext(PinAuthorizedUser))
            {
                twitterCtx.UpdateStatus(status, postId);
            }
        }

        /// <summary>
        /// Send an answer through twitter with the related question
        /// </summary>
        /// <param name="question"></param>
        /// <param name="answer">The answer given by expert, answer.Content + reference can't be longer then 140 characters</param>
        public void SendAnswer(QuestionEntity question, AnswerEntity answer)
        {
            var reference = question.Source.Source.Value;
            var postId = question.Source.PostId;

            var status = reference + " " + answer.Content;

            if (status.Length > 140)
            {
                // TODO create link to web answer page and make a resource containing the message: 'U antwoord kan hier gevonden worden:'.
                status = resourceManager.GetString(question.LanguageDefinition.ResourceName + "_TWITTER_LINK_RESPONSE");
            }

            validation.StringCheck(postId);
            validation.TweetLengthCheck(status);

            using (TwitterContext twitterCtx = new TwitterContext(PinAuthorizedUser))
            {
                twitterCtx.UpdateStatus(status, postId);
            }
        }
    }
}
