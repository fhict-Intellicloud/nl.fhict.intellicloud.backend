using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using nl.fhict.IntelliCloud.Common.CustomException; 

namespace nl.fhict.IntelliCloud.Business.Manager
{
    public class AnswerManager : BaseManager
    {
        
        public IList<Answer> GetAnswers(AnswerState answerState, int employeeId)
        {
            Validation.IdCheck(employeeId);

            List<Answer> answers = new List<Answer>();

            using (var ctx = new IntelliCloudContext())
            {

                List<AnswerEntity> answerentities = (from a in ctx.Answers
                                                         .Include(a => a.User)
                                                         .Include(a => a.User.Sources)        
                                                         .Include(a => a.User.Sources.Select(s => s.SourceDefinition))
                                                     where a.AnswerState == answerState
                                                     select a).ToList();

                answers = ConvertEntities.AnswerEntityListToAnswerList(answerentities);

            }

            return answers;

        }

        public Common.DataTransfer.Answer GetAnswer(string id)
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

                UserEntity user = ctx.Users.SingleOrDefault(ld => ld.Id == answererId);

                if (user == null)
                    throw new NotFoundException("No user entity exists with the specified ID.");

                answerEntity.User = user;
                // TODO link answer to question and generate a feedbackToken using GUID (can both be set in the question).
                // TODO set IsPrivate based on private settings in question.
                answerEntity.IsPrivate = false;

                // TODO determine real language 
                LanguageDefinitionEntity languageDefinition = ctx.LanguageDefinitions.SingleOrDefault(ld => ld.Name.Equals("English"));

                if (languageDefinition == null)
                    throw new NotFoundException("No languageDefinition entity exists with the specified ID.");

                answerEntity.LanguageDefinition = languageDefinition;

                ctx.Answers.Add(answerEntity);

                ctx.SaveChanges();

            }

            // TODO put the SendAnswerFactory in the BaseManager.
            // TODO send the answer using the this.SendAnswerFactory.LoadPlugin(question.Source.Source.SourDefinition).SendAnswer(question, answer) method.
        }

        public void UpdateAnswer(string id, AnswerState answerState)
        {
            Validation.IdCheck(id);
            // TODO how to process feedback on a question? Add Content as parameter?

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

                ctx.SaveChanges();

            }
        }
    }
}
