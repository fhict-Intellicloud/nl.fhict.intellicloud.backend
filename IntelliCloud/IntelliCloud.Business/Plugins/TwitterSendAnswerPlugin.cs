﻿using System.Resources;
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
        IValidation validation;

        /// <summary>
        /// Creates a new PinAutharizedUser
        /// Autorization needed since twitter api 1.1
        /// Account settings can be found in drive - Configuratie settings
        /// </summary>
        private PinAuthorizer _pinAutharizedUser;
        private PinAuthorizer PinAutharizedUser
        {
            get
            {
                if (_pinAutharizedUser == null)
                {
                    var auth = new PinAuthorizer
                    {
                        Credentials = new InMemoryCredentials
                        {
                            ConsumerKey = "5SFAC0n3LhszMHKvpDkvw",
                            ConsumerSecret = "TkP98l0xDl4FEucVq6WYfEAHyCgJi0b6IwSrOGfhCs",
                            OAuthToken = "2221459926-pUrExE5ls8d0m4D9rIkvSmL7a590XEzKElBOtrr",
                            AccessToken = "2eaC8UZsCdh9E5Pi0JebSZa04VwFFnahkuMf3NVYT41yd"
                        }
                    };

                    _pinAutharizedUser = auth;
                    return _pinAutharizedUser;
                }
                else
                {
                    return _pinAutharizedUser;
                }
            }
        }

        public TwitterSendAnswerPlugin()
        {
            validation = new Validation();
        }
        
        /// <summary>
        /// Sends a confirmation the the asker of the question
        /// </summary>
        /// <param name="question">the question asked by a user</param>
        public void SendQuestionRecieved(QuestionEntity question)
        {
            var reference = question.Source.Source.Value;
            var postId = question.Source.PostId;

            ResourceManager rm = Resources.ResourceManager;
            string tweetBody = rm.GetString(question.LanguageDefinition.ResourceName + "_TWITTER_AUTO_RESPONSE");

            var status = reference + " " + tweetBody;

            validation.StringCheck(postId);
            validation.TweetLengthCheck(status);

            using (TwitterContext twitterCtx = new TwitterContext(PinAutharizedUser))
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

            validation.StringCheck(postId);
            validation.TweetLengthCheck(status);

            using (TwitterContext twitterCtx = new TwitterContext(PinAutharizedUser))
            {
                twitterCtx.UpdateStatus(status, postId);
            }
        }
    }
}
