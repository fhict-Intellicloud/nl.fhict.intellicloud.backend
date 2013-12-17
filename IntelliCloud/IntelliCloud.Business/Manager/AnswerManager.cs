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
        /// Retrieves all the available answers and optionally filtering them using the answer state or employee 
        /// identifier.
        /// </summary>
        /// <param name="answerState">The optional answer state, only answers with this state will be returned.
        /// </param>
        /// <param name="employeeId">The optional employee identifier, only answers about which the employee has 
        /// knowledge are returned (keywords between user and answer match).</param>
        /// <returns>Returns the answers that match the filters.</returns>
        public IList<Answer> GetAnswers(AnswerState answerState, int? employeeId)
        {
            List<Answer> answers = new List<Answer>();

            using (var ctx = new IntelliCloudContext())
            {

                var query = (from a in ctx.Answers
                                                               .Include(a => a.User)
                                                               .Include(a => a.User.Sources)
                                                               .Include(a => a.User.Sources.Select(s => s.SourceDefinition))
                             select a).Where(x => x.AnswerState == answerState);

                if (employeeId != null)
                {
                    query.Where(x => x.User.Id == employeeId);
                }

                answers = ConvertEntities.AnswerEntityListToAnswerList(query.ToList());
            }

            return answers;

        }

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

                AnswerEntity answerentity = (from a in ctx.Answers
                                                         .Include(a => a.User)
                                                         .Include(a => a.User.Sources)
                                                         .Include(a => a.User.Sources.Select(s => s.SourceDefinition))
                                             where a.Id == iId
                                             select a).Single();

                answer = ConvertEntities.AnswerEntityToAnswer(answerentity);

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
                    throw new NotFoundException("No languageDefinition entity exists with the specified ID.");

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

                Question quest = ConvertEntities.QuestionEntityToQuestion(question);
                Answer a = ConvertEntities.AnswerEntityToAnswer(answerEntity);


                this.SendAnswerFactory.LoadPlugin(ConvertEntities.SourceDefinitionEntityToSourceDefinition(question.Source.Source.SourceDefinition))
                    .SendAnswer(quest, a);
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

                AnswerEntity answerEntity = (from a in ctx.Answers
                                                 .Include(a => a.User)
                                                 .Include(a => a.LanguageDefinition)
                                             where a.Id == iId
                                             select a).SingleOrDefault();

                if (answerEntity == null)
                    throw new NotFoundException("No answer entity exists with the specified ID.");

                answerEntity.AnswerState = answerState;
                answerEntity.Content = answer;
                answerEntity.LastChangedTime = DateTime.UtcNow;

                ctx.SaveChanges();

            }
        }
    }
}
