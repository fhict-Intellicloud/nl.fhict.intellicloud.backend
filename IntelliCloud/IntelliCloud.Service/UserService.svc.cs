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

        public System.Collections.Generic.IList<User> GetUsers(System.DateTime? after = null)
        {
            return this.manager.GetUsers(after);
        }

        public System.Collections.Generic.IList<Keyword> GetKeywords(string id)
        {
            return this.manager.GetKeywords(id);
        }

        public System.Collections.Generic.IList<Question> GetQuestions(string id, System.DateTime? after = null)
        {
            return this.manager.GetQuestions(id, after);
        }

        public System.Collections.Generic.IList<Answer> GetFeedbacks(string id, System.DateTime? after = null)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.IList<Answer> GetReviews(string id, System.DateTime? after = null)
        {
            throw new System.NotImplementedException();
        }

        public void AssignKeyword(string id, string keyword, int affinity)
        {
            throw new System.NotImplementedException();
        }
    }
}
