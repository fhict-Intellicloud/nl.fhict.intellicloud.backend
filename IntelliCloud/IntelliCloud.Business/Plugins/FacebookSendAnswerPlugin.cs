using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using System.Configuration;
using Facebook;
using System.Dynamic;

namespace nl.fhict.IntelliCloud.Business.Plugins.Loader
{
    class FacebookSendAnswerPlugin : ISendAnswerPlugin
    {
        private string accessToken = ConfigurationManager.AppSettings["IntelliCloud.Facebook.AccessToken"];
        private string pageId = ConfigurationManager.AppSettings["IntelliCloud.Facebook.PageId"];

        /// <summary>
        /// Sends answer through a comment on a facebook post
        /// </summary>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        public void SendAnswer(Question question, Answer answer)
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

        public void SendQuestionRecieved(Question question)
        {
            throw new NotImplementedException();
        }
    }
}
