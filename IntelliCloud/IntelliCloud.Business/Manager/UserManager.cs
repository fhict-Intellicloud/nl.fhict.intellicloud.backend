using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Common.DataTransfer.Input;
using System.Collections.Generic;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    /// <summary>
    /// Class used for handling service requests related to users.
    /// </summary>
    public class UserManager : BaseManager
    {
        /// <summary>
        /// Method for finding a user based on it's ID or a list of UserSource instances.
        /// All parameters are optional - if no parameters are provided the currently logged in user will be returned.
        /// </summary>
        /// <param name="id">The ID of the user. Optional.</param>
        /// <param name="sources">A list of UserSource instances. Optional.</param>
        /// <returns>The matched user based on the values of the parameters, otherwise the currently logged in user.</returns>
        public User GetUser(string id = null, IList<UserSource> sources = null)
        {
            // if (parameter != null) validation.ValidateMethod(parameter)

            return null;
        }

        /// <summary>
        /// Method used for creating a new user.
        /// </summary>
        /// <param name="type">The type of user. Required.</param>
        /// <param name="sources">A list of UserSource instances. Required (must contain at least one item).</param>
        /// <param name="firstName">The user's first name. Optional.</param>
        /// <param name="infix">The user's infix. Optional.</param>
        /// <param name="lastName">The user's last name. Optional.</param>
        public void CreateUser(UserType type, IList<UserSource> sources, string firstName = null, string infix = null, string lastName = null)
        {
            // if (parameter != null) validation.ValidateMethod(parameter)
        }

        /// <summary>
        /// Method used for updating an existing user. 
        /// </summary>
        /// <param name="id">The ID of the user to update. Required.</param>
        /// <param name="type">The new value indicating the type of user. Required.</param>
        /// <param name="sources">A list of sources to update. Optional.</param>
        /// <param name="firstName">The new first name value. Optional.</param>
        /// <param name="infix">The new infix value. Optional.</param>
        /// <param name="lastName">The new last name value. Optional.</param>
        public void UpdateUser(string id, UserType type, IList<UserSource> sources = null, string firstName = null, string infix = null, string lastName = null)
        {
            // if (parameter != null) validation.ValidateMethod(parameter)
        }
    }
}