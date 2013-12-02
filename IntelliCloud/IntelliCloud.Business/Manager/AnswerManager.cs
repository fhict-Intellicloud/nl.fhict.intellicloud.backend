using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                                                         .Include("User")
                                                         .Include("User.Sources")
                                                         .Include("User.Sources.SourceDefinition")
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
                                                         .Include("User")
                                                         .Include("User.Sources")
                                                         .Include("User.Sources.SourceDefinition")
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
                answerEntity.User = (ctx.Users.Single(u => u.Id == answererId));
                answerEntity.IsPrivate = false;
                answerEntity.LanguageDefinition = null;

                ctx.Answers.Add(answerEntity);

                ctx.SaveChanges();

            }
        }

        public void UpdateAnswer(string id, AnswerState answerState)
        {
            Validation.IdCheck(id);

            using (var ctx = new IntelliCloudContext())
            {
                int iId = Convert.ToInt32(id);

                AnswerEntity answerEntity = (from a in ctx.Answers
                                             where a.Id == iId
                                             select a).Single();

                answerEntity.AnswerState = answerState;

                ctx.SaveChanges();

            }
        }
    }
}
