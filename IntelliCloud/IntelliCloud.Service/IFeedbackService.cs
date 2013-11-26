using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace nl.fhict.IntelliCloud.Service
{
    [ServiceContract]
    public interface IFeedbackService
    {
        /// <summary>
        /// This method is used to accept answers
        /// </summary>
        /// <param name="feedback">Use this parameter if the answer is correct but still can use some tweaking</param>
        /// <param name="answerId">The Id of the answer you want to accept</param>
        /// <param name="questionId">The Id of the question where this answer is accepted for</param>
        /// <returns>Returns whether the accept succedeed of failed</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "AcceptAnswer",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void AcceptAnswer(String feedback, String answerId, String questionId);

        /// <summary>
        /// This method is used to decline answers
        /// </summary>
        /// <param name="feedback">Use this parameter to fill in wath is missing for this answer to be correct</param>
        /// <param name="answerId">The Id of the answer you want to decline</param>
        /// <param name="questionId">The Id of the question where this answer is declined for</param>
        /// <returns>Returns whether the decline succeeded or failed</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "DeclineAnswer",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void DeclineAnswer(String feedback, String answerId, String questionId);
    }
}
