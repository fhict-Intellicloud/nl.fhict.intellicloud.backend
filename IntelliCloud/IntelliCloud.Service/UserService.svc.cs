using nl.fhict.IntelliCloud.Business.Manager;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Common.DataTransfer.Input;
using System.Collections.Generic;

namespace nl.fhict.IntelliCloud.Service
{
    /// <summary>
    /// A service providing functionality related to users.
    /// </summary>
    public class UserService : IUserService
    {
        /// <summary>
        /// Field that contains an instance of class UserManager.
        /// </summary>
        private readonly UserManager manager;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UserService()
        {
            this.manager = new UserManager();
        }

        /// <summary>
        /// Method for getting a user based on it's ID or a list of UserSource instances.
        /// All parameters are optional - if no parameters are provided the currently logged in user will be returned.
        /// </summary>
        /// <param name="id">The ID of the user. Optional.</param>
        /// <param name="sources">A list of UserSource instances. Optional.</param>
        /// <returns>The matched user based on the values of the parameters, otherwise the currently logged in user.</returns>
        public User GetUser(string userId, List<UserSource> sources)
        {
            return this.manager.GetUser(userId, sources);
        }

        /// <summary>
        /// Method used for creating a new user.
        /// </summary>
        /// <param name="type">The type of user. Required.</param>
        /// <param name="sources">A list of UserSource instances. Required (must contain at least one item).</param>
        /// <param name="firstName">The user's first name. Optional.</param>
        /// <param name="infix">The user's infix. Optional.</param>
        /// <param name="lastName">The user's last name. Optional.</param>
        /// <param name="avatar">The user's avatar URL. Optional.</param>
        public void CreateUser(UserType type, IList<UserSource> sources, string firstName, string infix, string lastName, string avatar)
        {
            this.manager.CreateUser(type, sources, firstName, infix, lastName, avatar);
        }
    }
}
