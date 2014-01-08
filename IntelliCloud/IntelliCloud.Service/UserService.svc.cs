using System;
using System.Collections.Generic;
using nl.fhict.IntelliCloud.Business.Manager;
using nl.fhict.IntelliCloud.Common.DataTransfer;

namespace nl.fhict.IntelliCloud.Service
{
    /// <summary>
    /// A service providing functionality related to users.
    /// </summary>
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

        public IList<User> GetUsers(DateTime? after = null)
        {
            return this.manager.GetUsers(after);
        }

        public IList<Keyword> GetKeywords(string id)
        {
            return this.manager.GetKeywords(id);
        }

        public IList<Question> GetQuestions(string id, DateTime? after = null)
        {
            return this.manager.GetQuestions(id, after);
        }

        public IList<Answer> GetFeedbacks(string id, DateTime? after = null)
        {
            return this.manager.GetFeedbacks(id, after);
        }

        public IList<Answer> GetReviews(string id, DateTime? after = null)
        {
            return this.manager.GetReviews(id, after);
        }

        public void AssignKeyword(string id, string keyword, int affinity)
        {
            this.manager.AssignKeyword(id, keyword, affinity);
        }
    }
}
