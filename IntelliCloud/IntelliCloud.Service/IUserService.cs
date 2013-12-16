using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using nl.fhict.IntelliCloud.Business.Authorization;
using nl.fhict.IntelliCloud.Common.DataTransfer;

namespace nl.fhict.IntelliCloud.Service
{
    [ServiceContract]
    public interface IUserService
    {

        /// <summary>
        /// This method returns the user that is logged on
        /// </summary>
        /// <param name="userId">This parameter is used to get a specific user object</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "users/{userId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired]
        User GetUser(String userId);
    }
}
