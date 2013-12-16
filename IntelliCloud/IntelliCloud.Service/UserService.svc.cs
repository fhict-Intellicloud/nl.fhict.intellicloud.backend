using nl.fhict.IntelliCloud.Business.Manager;
using nl.fhict.IntelliCloud.Common.DataTransfer;

namespace nl.fhict.IntelliCloud.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager manager;

        public UserService()
        {
            this.manager = new UserManager();
        }

        public User GetUser(string userId)
        {
            return this.manager.GetUser(userId);
        }
    }
}
