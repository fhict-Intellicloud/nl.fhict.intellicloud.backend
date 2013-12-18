using nl.fhict.IntelliCloud.Business.Authorization;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Common.DataTransfer.Input;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace nl.fhict.IntelliCloud.Service
{
    [ServiceContract]
    public interface IUserService
    {
        /// <summary>
        /// Method for getting a user based on it's ID or a list of UserSource instances.
        /// All parameters are optional - if no parameters are provided the currently logged in user will be returned.
        /// </summary>
        /// <param name="id">The ID of the user. Optional.</param>
        /// <param name="sources">A list of UserSource instances. Optional.</param>
        /// <returns>The matched user based on the values of the parameters, otherwise the currently logged in user.</returns>
        [OperationContract]
        [WebInvoke(Method = "PUT",
            UriTemplate = "users?userId={userId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        [AuthorizationRequired(UserType.Customer, UserType.Employee)]
        User GetUser(string userId, IList<UserSource> sources);

        /// <summary>
        /// Method used for creating a new user.
        /// </summary>
        /// <param name="type">The type of user. Required.</param>
        /// <param name="sources">A list of UserSource instances. Required (must contain at least one item).</param>
        /// <param name="firstName">The user's first name. Optional.</param>
        /// <param name="infix">The user's infix. Optional.</param>
        /// <param name="lastName">The user's last name. Optional.</param>
        /// <param name="avatar">The user's avatar URL. Optional.</param>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "users",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        [AuthorizationRequired(UserType.Employee)]
        void CreateUser(UserType type, IList<UserSource> sources, string firstName, string infix, string lastName, string avatar);
    }
}
