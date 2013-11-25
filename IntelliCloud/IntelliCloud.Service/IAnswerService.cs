using nl.fhict.IntelliCloud.Common.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace nl.fhict.IntelliCloud.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAnswerService" in both code and config file together.
    [ServiceContract]
    public interface IAnswerService
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

        /// <summary>
        /// This method is used to get all the Answers that have to be reviewed by a given employee.
        /// </summary>
        /// <param name="employeeId">The Id of the employee you want to get the answers from</param>
        /// <returns>Return a list containing all the answers that have to be reviewed by the employee</returns>
        [OperationContract]
        [WebGet(UriTemplate = "GetAnswersUpForReview/{employeeId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        List<Answer> GetAnswersUpForReview(String employeeId);

        /// <summary>
        /// This method is used to get the answer by the answerId
        /// </summary>
        /// <param name="answerId">This Id of the answer you want to recieve</param>
        /// <returns>Returns the answer you want to use</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "GetAnswerById/{answerId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Answer GetAnswerById(String answerId);
    }
}
