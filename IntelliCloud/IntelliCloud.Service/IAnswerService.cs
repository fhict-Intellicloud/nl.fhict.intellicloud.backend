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
    [ServiceContract]
    public interface IAnswerService
    {
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
        [WebGet(UriTemplate = "GetAnswerById/{answerId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Answer GetAnswerById(String answerId);
    }
}
