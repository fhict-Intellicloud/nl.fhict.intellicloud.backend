using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using nl.fhict.IntelliCloud.Common.CustomException;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    /// <summary>
    /// A class providing functionality related to answers.
    /// </summary>
    public class AnswerManager : BaseManager
    {
        /// <summary>
        /// Constructor method for the answer manager class.
        /// </summary>
        public AnswerManager()
            : base()
        { }

        /// <summary>
        /// Constructor class for the answer manager.
        /// </summary>
        /// <param name="validation">An instance of <see cref="IValidation"/>.</param>
        public AnswerManager(IValidation validation)
            : base(validation)
        { }

        /// <summary>
        /// Retrieve the answer with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the answer.</param>
        /// <returns>Returns the answer with the given identifier.</returns>
        public Answer GetAnswer(string id)
        {
            Validation.IdCheck(id);

            Answer answer = new Answer();

            using (var ctx = new IntelliCloudContext())
            {
                int iId = Convert.ToInt32(id);

                AnswerEntity answerentity = ctx.Answers
                    .Include(a => a.LanguageDefinition)
                    .Single(a => a.Id == iId);

                answer = answerentity.AsAnswer();
            }

            return answer;

        }

        /// <summary>
        /// Creates a new answer.
        /// </summary>
        /// <param name="questionId">The identifier of the question which is answered.</param>
        /// <param name="answer">The content of the given answer.</param>
        /// <param name="answererId">The employee who answered the question.</param>
        /// <param name="answerState">The state of the answer.</param>
        public void CreateAnswer(int questionId, string answer, int answererId, AnswerState answerState)
        {
            Validation.IdCheck(answererId);
            Validation.IdCheck(questionId);
            Validation.StringCheck(answer);

            using (var ctx = new IntelliCloudContext())
            {

                AnswerEntity answerEntity = new AnswerEntity();

                answerEntity.AnswerState = answerState;
                answerEntity.Content = answer;
                answerEntity.CreationTime = DateTime.UtcNow;
                answerEntity.LastChangedTime = DateTime.UtcNow;

                UserEntity user = ctx.Users
                    .Include(u => u.Sources)
                    .SingleOrDefault(ld => ld.Id == answererId);

                if (user == null)
                    throw new NotFoundException("No user entity exists with the specified ID.");

                answerEntity.User = user;

                // TODO determine real language 
                LanguageDefinitionEntity languageDefinition = ctx.LanguageDefinitions.SingleOrDefault(ld => ld.Name.Equals("English"));

                if (languageDefinition == null)
                    throw new NotFoundException("No languageDefinition entity exists with the specified name.");

                answerEntity.LanguageDefinition = languageDefinition;

                ctx.Answers.Add(answerEntity);

                ctx.SaveChanges();

                QuestionEntity question = ctx.Questions
                    .Include(q => q.User)
                    .Include(q => q.User.Sources)
                    .Include(q => q.Source)
                    .Include(q => q.Source.Source)
                    .Include(q => q.Source.Source.SourceDefinition)
                    .Single(q => q.Id == questionId);

                question.Answer = answerEntity;
                question.Answerer = user;

                Guid token = Guid.NewGuid();
                question.FeedbackToken = token.ToString();

                answerEntity.IsPrivate = question.IsPrivate;

                ctx.SaveChanges();

                this.SendAnswerFactory.LoadPlugin(question.Source.Source.SourceDefinition).SendAnswer(question, answerEntity);
            }

        }

        /// <summary>
        /// Updates the answer with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the answer that is updated.</param>
        /// <param name="answerState">The new state of the answer.</param>
        /// <param name="answer">The new content of the given answer.</param>
        public void UpdateAnswer(string id, AnswerState answerState, string answer)
        {
            Validation.IdCheck(id);
            Validation.StringCheck(answer);

            using (var ctx = new IntelliCloudContext())
            {
                int iId = Convert.ToInt32(id);

                AnswerEntity answerEntity = ctx.Answers
                    .Include(a => a.LanguageDefinition)
                    .Include(a => a.User)
                    .SingleOrDefault(a => a.Id == iId);

                if (answerEntity == null)
                    throw new NotFoundException("No answer entity exists with the specified ID.");

                answerEntity.AnswerState = answerState;
                answerEntity.Content = answer;
                answerEntity.LastChangedTime = DateTime.UtcNow;

                ctx.SaveChanges();

            }
        }

        public IList<Answer> GetAnswers(AnswerState state, string search)
        {
            Validation.StringCheck(search);
            
            // TODO implement Daniks algorithm
            throw new NotImplementedException();
        }

        public User GetAnswerer(string id)
        {
            Validation.IdCheck(id);

            User user = new User();

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                int iId = Convert.ToInt32(id);

                AnswerEntity answerEntity = ctx.Answers
                    .Include(a => a.User)
                    .Include(a => a.User.Sources)
                    .SingleOrDefault(a => a.Id == iId);

                if (answerEntity == null)
                    throw new NotFoundException("No answer found for the given Id");

                user = answerEntity.User.AsUser();
            }

            return user;
        }

        public IList<Feedback> GetFeedbacks(string id, FeedbackState? state)
        {
            Validation.IdCheck(id);

            List<Feedback> feedbacks = new List<Feedback>();

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                int iId = Convert.ToInt32(id);
                
                var query = ctx.Feedbacks.Where(f => f.Answer.Id == iId);

                if (state != null)
                {
                    query = query.Where(f => f.FeedbackState == state);
                }

                List<FeedbackEntity> feedbackEntities = query.ToList();

                feedbacks.AddRange(feedbackEntities.AsFeedbacks());
            }

            return feedbacks;
        }

        public IList<Review> GetReviews(string id, ReviewState? state)
        {
            Validation.IdCheck(id);

            List<Review> reviews = new List<Review>();

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                int iId = Convert.ToInt32(id);

                var query = ctx.Reviews.Where(f => f.Answer.Id == iId);

                if (state != null)
                {
                    query = query.Where(f => f.ReviewState == state);
                }

                List<ReviewEntity> reviewEntities = query.ToList();

                reviews.AddRange(reviewEntities.AsReviews());
            }

            return reviews;
        }

        public IList<Keyword> GetKeywords(string id)
        {
            Validation.IdCheck(id);

            List<Keyword> keywords = new List<Keyword>();

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                int iId = Convert.ToInt32(id);

                List<KeywordEntity> keywordEntities = (from k in ctx.Keywords
                                                      join ak in ctx.AnswerKeys
                                                      on k.Id equals ak.Keyword.Id
                                                      select k).ToList();

                keywords.AddRange(keywordEntities.AsKeywords());
            }

            return keywords;
        }
    }
}
