using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using System.IO;

namespace IntelliCloud
{
    [ServiceContract]
    public interface IIntelliCloud
    {
        /// <summary>
        /// This method is used to ask a question to a employee.
        /// </summary>
        /// <param name="source">the name of the source as the source is known in the database</param>
        /// <param name="reference">the reference needed by the plugin to send the answers back</param>
        /// <param name="question">the question itself</param>
        /// <returns>Returns whether the question upload was succesfull or failed</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", 
            UriTemplate = "AskQuestion/{source}/{reference}/{question}", 
            RequestFormat = WebMessageFormat.Json, 
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void AskQuestion(String source, String reference, String question);

        /// <summary>
        /// This method is used to send an answer directly back to the asker, in this case there won't be any reviewing
        /// </summary>
        /// <param name="questionId">The questionId of the question that this answer answers</param>
        /// <param name="answerId">The Id of the answer</param>
        /// <returns>Returns whether the send was succesfull or failed</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", 
            UriTemplate = "SendAnswer", 
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void SendAnswer(String questionId, String answerId);

        /// <summary>
        /// This method is used to create an answer.
        /// </summary>
        /// <param name="questionId">The questionId of the question that this answer answers</param>
        /// <param name="answer">The answer itself</param>
        /// <param name="answererId">The id of the user who answered it</param>
        /// <param name="answerState">The answerstate it should get</param>
        /// <returns>Returns whether the create was succesfull or failed</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "CreateAnswer",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void CreateAnswer(String questionId, String answer, String answererId, string answerState);

        /// <summary>
        /// This method is used to update the answerstate of an answer.
        /// </summary>
        /// <param name="answerId">The answerId of the answer that has to be updated</param>
        /// <param name="answerState">The answerstate it should be updated to</param>
        /// <returns>Returns whether the update was succesfull or failed</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "UpdateAnswer",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void UpdateAnswer(String answerId, String answerState);

        /// <summary>
        /// This method return the question availible to this employee
        /// </summary>
        /// <param name="employeeId">The employeeId of the employee who requests the questions</param>
        /// <returns>Returns the questions availible to this employee</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", 
            UriTemplate = "GetQuestions/{employeeId}", 
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<Question> GetQuestions(String employeeId);

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
        /// This method is used to send a review for a specific answer
        /// </summary>
        /// <param name="reviewerId">The Id of the employee who wrote the review</param>
        /// <param name="answerId">The Id of the answer this review is written for</param>
        /// <param name="review">The review text itself</param>
        /// <returns>Return whether the review was recieved by the server</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", 
            UriTemplate = "SendReviewForAnswer", 
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void SendReviewForAnswer(String reviewerId, String answerId, String review);

        /// <summary>
        /// This method is used to update the reviewstate of an review.
        /// </summary>
        /// <param name="reviewId">The reviewId of the review that has to be updated</param>
        /// <param name="reviewState">The reviewstate it should be updated to</param>
        /// <returns>Returns whether the update was succesfull or failed</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "UpdateReview",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void UpdateReview(String reviewId, String reviewState);

        /// <summary>
        /// This method is used to get all the Reviews written for a specific answer
        /// </summary>
        /// <param name="answerId">The Id of the answer you want to get the reviews for</param>
        /// <returns>Return a list containing all the reviews for this specified answer</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", 
            UriTemplate = "GetReviewsForAnswer/{answerId}", 
            RequestFormat = WebMessageFormat.Json, 
            ResponseFormat = WebMessageFormat.Json)]
        List<Review> GetReviewsForAnswer(String answerId);

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

        /// <summary>
        /// This method is used to get the question by the questionId
        /// </summary>
        /// <param name="questionId">this Id of the quetion you want to recieve</param>
        /// <returns>Returns the question you want to use</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "GetQuestionById/{questionId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Question GetQuestionById(String questionId);

    }

}
