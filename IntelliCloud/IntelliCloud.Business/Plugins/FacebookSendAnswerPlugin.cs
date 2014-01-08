using System;
using System.Resources;
using nl.fhict.IntelliCloud.Business.Plugins.Loader;
using nl.fhict.IntelliCloud.Business.Properties;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using System.Configuration;
using Facebook;
using System.Dynamic;

namespace nl.fhict.IntelliCloud.Business.Plugins
{
    public class FacebookSendAnswerPlugin : ISendAnswerPlugin
    {
        private string accessToken = ConfigurationManager.AppSettings["IntelliCloud.Facebook.AccessToken"];
        private string pageId = ConfigurationManager.AppSettings["IntelliCloud.Facebook.PageId"];
        
        /// <summary>
        /// Sends a confirmation the the asker of the question
        /// </summary>
        /// <param name="question">the question asked by a user</param>
        public void SendQuestionReceived(Data.IntelliCloud.Model.QuestionEntity question)
        {
            //Get the id of the post containing the question
            String postId = question.Source.PostId;

            //Create a new dynamic object that contains the content of the answer
            dynamic parameters = new ExpandoObject();
            ResourceManager rm = Resources.ResourceManager;
            string facobookPostBody = rm.GetString(question.LanguageDefinition.ResourceName + "_FACEBOOK_AUTO_RESPONSE");

            parameters.message = facobookPostBody;

            //Create a new facebook client
            FacebookClient facebookClient = new FacebookClient(accessToken);

            //Comment to the post containing the question
            facebookClient.Post("/" + postId + "/comments", parameters);
        }

        /// <summary>
        /// Sends answer through a comment on a facebook post
        /// </summary>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        public void SendAnswer(Data.IntelliCloud.Model.QuestionEntity question, Data.IntelliCloud.Model.AnswerEntity answer)
        {
            //Get the id of the post containing the question
            String postId = question.Source.PostId;

            //Create a new dynamic object that contains the content of the answer
            dynamic parameters = new ExpandoObject();
            parameters.message = answer.Content;

            //Create a new facebook client
            FacebookClient facebookClient = new FacebookClient(accessToken);

            //Comment to the post containing the question
            facebookClient.Post("/" + postId + "/comments", parameters);
        }
    }
}
