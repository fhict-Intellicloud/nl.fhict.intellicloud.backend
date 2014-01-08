using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using nl.fhict.IntelliCloud.Common.CustomException;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;
using nl.fhict.IntelliCloud.Data.WordStoreService;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    /// <summary>
    /// A class providing functionality related to questions.
    /// </summary>
    public class QuestionManager : BaseManager
    {
        /// <summary>
        /// Constructor method for the question manager class.
        /// </summary>
        public QuestionManager()
            : base()
        {
        }

        /// <summary>
        /// Constructor class for the question manager.
        /// </summary>
        /// <param name="validation">An instance of <see cref="IValidation"/>.</param>
        public QuestionManager(IValidation validation)
            : base(validation)
        { }



        /// <summary>
        /// Retrieve the question with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the question.</param>
        /// <returns>Returns the question with the given identifier.</returns>
        public Question GetQuestion(int id)
        {
            Validation.IdCheck(id);

            Question question = new Question();

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                // TODO: make sure only users of type employee can retrieve private questions.
                QuestionEntity entity = (from q in ctx.Questions
                                                                 .Include(q => q.Source)
                                                                 .Include(q => q.User)
                                                                 .Include(q => q.User.Sources)
                                                                 .Include(q => q.Answerer)
                                                                 .Include(q => q.Answerer.Sources)
                                                                 .Include(q => q.User.Sources.Select(s => s.SourceDefinition))
                                         where q.Id == id
                                         select q).SingleOrDefault();

                if (entity == null)
                    throw new NotFoundException("No Question entity exists with the specified ID.");

                question = entity.AsQuestion();
            }
            return question;
        }

        /// <summary>
        /// Creates a new question.
        /// </summary>
        /// <param name="source">The source from which the question was send, e.g. "Mail", "Facebook" or "Twitter".
        /// </param>
        /// <param name="reference">The identifier for the source, e.g. the username or email address.</param>
        /// <param name="question">The question that was answered.</param>
        /// <param name="title">The title of the question. The title contains a short summary of the question.</param>
        /// <param name="postId">The identifier of the post this question originates from, for example the Facebook post
        /// identifier.</param>
        /// <param name="isPrivate">When <c>true</c> the question is private, otherwise the question is public. Private 
        /// questions are only available to employees and will never be exposed to customers.</param>
        public void CreateQuestion(
            string source, string reference, string question, string title, string postId = null, bool isPrivate = false)
        {
            Validation.StringCheck(source);
            Validation.StringCheck(reference);
            Validation.StringCheck(question);
            Validation.StringCheck(title);

            List<KeywordEntity> addedKeywords = new List<KeywordEntity>();
            IList<Word> words = this.ResolveWords(question);
            Language language = this.GetLanguage(words);
            //var keywordScores = this.GetKeywordScores(words, language);


            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                UserEntity userEntity = this.GetUserEntity(source, reference);
                SourceEntity sourceDef = ResolveSource(source, userEntity.Id);

                //Add method to get or create keywords in database.
                List<String> keys = new List<string>();
                keys = words.Select(x => x.Value).Distinct().ToList();
                List<KeywordEntity> keyEntities = new List<KeywordEntity>();
                keyEntities = ctx.Keywords.Where(t => keys.Contains(t.Name)).ToList();
                foreach (Word word in words)
                {
                    if (keyEntities.FirstOrDefault(x => x.Name == word.Value) == null)
                    {
                        KeywordEntity key = new KeywordEntity();
                        key.Name = word.Value;
                        key.CreationTime = DateTime.Now;
                        ctx.Keywords.Add(key);
                        addedKeywords.Add(key);
                    }
                }
                if (addedKeywords.Count != 0)
                {
                    ctx.SaveChanges();
                }


                QuestionEntity questionEntity = new QuestionEntity()
                {
                    Content = question,
                    CreationTime = DateTime.UtcNow,
                    IsPrivate = isPrivate,
                    QuestionState = QuestionState.Open,
                    Title = title,
                    Source = new QuestionSourceEntity()
                    {
                        Source = sourceDef,
                        PostId = postId
                    },
                    LanguageDefinition = ctx.LanguageDefinitions.Single(x => x.Name == language.ToString()),
                    User = userEntity
                };

                ctx.Questions.Add(questionEntity);

                //Link keyword entities with affinity to question. using questionkeyentity
                List<KeywordEntity> questionKeys = new List<KeywordEntity>();
                List<QuestionKeyEntity> questionKeywords = new List<QuestionKeyEntity>();
                questionKeys.AddRange(addedKeywords);//New keywords.
                questionKeys.AddRange(keyEntities);//Already exsisting keywords.

                foreach (KeywordEntity k in questionKeys)
                {
                    QuestionKeyEntity ent = new QuestionKeyEntity();
                    ent.Keyword = k;
                    ent.Question = questionEntity;
                    ent.CreationTime = DateTime.Now;
                    ent.Affinity = words.Where(x => x.Value == k.Name).Count();
                }
                ctx.SaveChanges();

                // TODO check if there is a 90%+  match - ???
                Boolean match = false;


                // Send auto response
                if (!match)
                {
                    this.SendAnswerFactory.LoadPlugin(questionEntity.Source.Source.SourceDefinition)
                        .SendQuestionRecieved(questionEntity);
                }

            }
        }

        /// <summary>
        /// Updates the question with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the question that is updated.</param>
        /// <param name="employeeId">The identifier of the employee that is going to answer the question.</param>
        public void UpdateQuestion(int id, int employeeId)
        {
            Validation.IdCheck(id);
            Validation.IdCheck(employeeId);

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                QuestionEntity questionEntity = (from q in ctx.Questions
                                                 where q.Id == id
                                                 select q).Single();

                questionEntity.Answerer = (from u in ctx.Users
                                           where u.Id == employeeId
                                           select u).Single();
                questionEntity.LastChangedTime = DateTime.UtcNow;

                ctx.SaveChanges();
            }

        }

        /// <summary>
        /// Retrieve the question for this feedback token.
        /// </summary>
        /// <param name="feedbackToken">The feedback token of the question.</param>
        /// <returns>Returns the question with the given feedbacktoken.</returns>
        public Question GetQuestionByFeedbackToken(string feedbackToken)
        {
            Validation.StringCheck(feedbackToken);

            Question question = new Question();

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                QuestionEntity entity = (from q in ctx.Questions
                                                                 .Include(q => q.Source)
                                                                 .Include(q => q.User)
                                                                 .Include(q => q.User.Sources)
                                                                 .Include(q => q.Answerer)
                                                                 .Include(q => q.Answerer.Sources)
                                                                 .Include(q => q.User.Sources.Select(s => s.SourceDefinition))
                                         where q.FeedbackToken == feedbackToken
                                         select q).SingleOrDefault();

                if (entity == null)
                    throw new NotFoundException("No Question entity exists with the specified feedback token.");

                question = entity.AsQuestion();
            }
            return question;
        }

        #region keyword algorith methods

        internal SourceEntity ResolveSource(string source, int userid)
        {
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                SourceEntity entity = (from q in ctx.Sources
                                                                 .Include(q => q.CreationTime)
                                                                 .Include(q => q.SourceDefinition)
                                                                 .Include(q => q.Id)
                                                                 .Include(q => q.UserId)
                                                                 .Include(q => q.User.Avatar)
                                                                 .Include(q => q.User.CreationTime)
                                                                 .Include(q => q.User.FirstName)
                                                                 .Include(q => q.User.Id)
                                                                 .Include(q => q.User.Infix)
                                                                 .Include(q => q.User.LastChangedTime)
                                                                 .Include(q => q.User.LastName)
                                                                 .Include(q => q.User.Sources)
                                                                 .Include(q => q.User.Type)
                                                                 .Include(q => q.Value)
                                       where q.SourceDefinition.Name == source && q.UserId == userid
                                       select q).SingleOrDefault();

                if (entity == null)
                    throw new NotFoundException("No SourceEntity entity exists with the specified source.");
                return entity;
            }
        }

        /// <summary>
        /// A function in which all punctuation is removed and all text is converted to lowercase.
        /// Words with punctuation within them are ignored (eg. andré, hbo'er).
        /// </summary>
        /// <param name="question">A string representing a question that is to be decomposed.</param>
        /// <returns> A list with each word in the sentence is returned. </returns>
        internal IList<string> ConvertQuestion(String question)
        {
            //Regex that matches every single special character exluding the -
            Regex regex = new Regex("[\\\\+=`~!@#$%^&*()_\\\\\\\\[\\]{}:\\\";\\?<>/.,|]");

            string cleanQuestion = regex.Replace(question.ToLowerInvariant(), "");

            return cleanQuestion.Split(' ')
                .Where(x => x != "-" && x != "'")
                .ToList();
        }

        /// <summary>
        /// Function that evaluates all words in the given question to its wordtypes.
        /// All verbs are converted to their full version. (eg worked --> to work, werkte --> werken)
        /// </summary>
        /// <param name="question">A given question for which all words are to be resolved to their type.</param>
        /// <returns>A list containing the resolved words that were contained in the question</returns>
        internal IList<Word> ResolveWords(String question)
        {
            IWordService service = new WordServiceClient();
            return ConvertQuestion(question)
                .SelectMany(x => service.ResolveWord(x))
                .ToList();
        }

        /// <summary>
        /// Function that finds the most likely keywords from a given question. This is done by returning all Nouns, Pronouns and Verbs. 
        /// </summary>
        /// <param name="question">A question from which one needs the keywords.</param>
        /// <param name="language">The language one needs the found keywords of. </param>
        /// <returns>Returns a List containing the most likely keywords from a given question.</returns>
        internal IList<Word> FindMostLikelyKeywords(IList<Word> words, Language language)
        {
            return words
                .Where(x =>
                    (x.Type == WordType.Noun || x.Type == WordType.Verb || x.Type == WordType.Pronoun)
                    && x.Language == language)
                .ToList();
        }

        /// <summary>
        /// Function to determine the langauge of set of words. 
        /// </summary>
        /// <param name="words">Set of words from which one needs to determine the language.</param>
        /// <returns>Returns the language that is the most common within the given set.</returns>
        internal Language GetLanguage(IList<Word> words)
        {
            var distinctLanguages = words
                .Where(x => x.Language != Language.Unknown)
                .GroupBy(x => x.Language)
                .Select(x => new { Language = x.Key, Count = x.Distinct().Count() });

            return distinctLanguages.Any()
                ? distinctLanguages.Single(x => x.Count == distinctLanguages.Max(y => y.Count)).Language
                : Language.Unknown;
        }


        private UserEntity GetUserEntity(string source, string reference)
        {
            using (var context = new IntelliCloudContext())
            {
                SourceDefinitionEntity sourceDefinition = context.SourceDefinitions
                    .SingleOrDefault(sd => sd.Name == source);

                if (sourceDefinition == null)
                    throw new NotFoundException("The provided source doesn't exists.");

                // Check if the user already exists
                SourceEntity sourceEntity = context.Sources
                    .SingleOrDefault(s => s.SourceDefinition.Id == sourceDefinition.Id && s.Value == reference);

                UserEntity userEntity;
                if (sourceEntity != null)
                {
                    // user already has an account, use this
                    userEntity = context.Users.Single(u => u.Id == sourceEntity.UserId);
                }
                else
                {
                    // user has no account, create one
                    userEntity = new UserEntity()
                    {
                        CreationTime = DateTime.UtcNow,
                        Type = UserType.Customer
                    };

                    context.Users.Add(userEntity);

                    // Mount the source to the new user
                    sourceEntity = new SourceEntity()
                    {
                        Value = reference,
                        CreationTime = DateTime.UtcNow,
                        SourceDefinition = sourceDefinition,
                        User = userEntity,
                    };

                    context.Sources.Add(sourceEntity);
                }
                return userEntity;
            }
        }
        #endregion

        public void UpdateQuestion(string id, int employeeId)
        {
            int intId = Convert.ToInt32(id);
            this.UpdateQuestion(id, employeeId);
        }

        public Question GetQuestion(string id)
        {
            int intId = Convert.ToInt32(id);
            return this.GetQuestion(intId);
        }

        public IList<Question> GetQuestions(int employeeId, QuestionState? state)
        {
            List<Question> questions = new List<Question>();
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {

                IList<QuestionEntity> questionEntities = (from q in ctx.Questions
                                                                 .Include(q => q.Source)
                                                                 .Include(q => q.User)
                                                                 .Include(q => q.User.Sources)
                                                                 .Include(q => q.Answerer)
                                                                 .Include(q => q.Answerer.Sources)
                                                                 .Include(q => q.User.Sources.Select(s => s.SourceDefinition))
                                                          where q.Answer == null || q.Answer.AnswerState != AnswerState.Ready
                                                          select q).ToList();
                if (employeeId != 0)
                {

                    List<QuestionEntity> openEmployeeQuestions = null;
                    List<QuestionEntity> nonEmployeeQuestions = null;

                    if (state == null)
                    {
                        openEmployeeQuestions = (from questionkeys in ctx.QuestionKeys
                                                 where
                                                     (from userkeys in ctx.UserKeys where userkeys.User.Id == employeeId select userkeys).Select(keyid => keyid.Keyword.Id).Contains(questionkeys.Keyword.Id)
                                                 select questionkeys).Select(x => x.Question).Where(x => x.QuestionState != QuestionState.Closed).Distinct().ToList();
                        nonEmployeeQuestions = (from questionkeys in ctx.QuestionKeys select questionkeys).Except((from questionkeys in ctx.QuestionKeys
                                                                                                                   where
                                                                                                                       (from userkeys in ctx.UserKeys select userkeys).Select(keyid => keyid.Keyword.Id).Contains(questionkeys.Keyword.Id)
                                                                                                                   select questionkeys)).Select(q => q.Question).Include(q => q.Source)
                                                             .Include(q => q.User)
                                                             .Include(q => q.User.Sources)
                                                             .Include(q => q.Answerer)
                                                             .Include(q => q.Answerer.Sources)
                                                             .Include(q => q.User.Sources.Select(s => s.SourceDefinition)).Where(x => x.QuestionState == state).Distinct().ToList();
                    }
                    else
                    {
                        openEmployeeQuestions = (from questionkeys in ctx.QuestionKeys
                                                 where
                                                     (from userkeys in ctx.UserKeys where userkeys.User.Id == employeeId select userkeys).Select(keyid => keyid.Keyword.Id).Contains(questionkeys.Keyword.Id)
                                                 select questionkeys).Select(x => x.Question).Where(x => x.QuestionState != QuestionState.Closed).Distinct().ToList();
                        nonEmployeeQuestions = (from questionkeys in ctx.QuestionKeys select questionkeys).Except((from questionkeys in ctx.QuestionKeys
                                                                                                                   where
                                                                                                                       (from userkeys in ctx.UserKeys select userkeys).Select(keyid => keyid.Keyword.Id).Contains(questionkeys.Keyword.Id)
                                                                                                                   select questionkeys)).Select(q => q.Question).Include(q => q.Source)
                                                             .Include(q => q.User)
                                                             .Include(q => q.User.Sources)
                                                             .Include(q => q.Answerer)
                                                             .Include(q => q.Answerer.Sources)
                                                             .Include(q => q.User.Sources.Select(s => s.SourceDefinition)).Where(x => x.QuestionState == state).Distinct().ToList();
                    }

                    List<QuestionEntity> totalEmplyeeList = new List<QuestionEntity>();
                    totalEmplyeeList.AddRange(openEmployeeQuestions);
                    totalEmplyeeList.AddRange(nonEmployeeQuestions);



                    //List<QuestionEntity> employeeQuestions = new List<QuestionEntity>();
                    //List<QuestionEntity> employeeNotFoundQuestions = new List<QuestionEntity>();


                    //List<UserKeyEntity> employeeKeys = (from x in ctx.UserKeys where x.User.Id == employee.Id select x)
                    //    .Include(q => q.User)
                    //    .Include(q => q.Keyword)
                    //    .Include(q => q.Affinity).ToList();
                    //List<QuestionKeyEntity> questionKeyEntities = (from q in ctx.QuestionKeys select q)
                    //    .Include(q => q.Question)
                    //    .Include(q => q.Keyword)
                    //    .Include(q => q.Affinity).ToList();

                    ////Check if employee keys contains questions keys.
                    //foreach (QuestionEntity question in questionEntities)
                    //{
                    //    bool foundUserQuestion = false;
                    //    List<QuestionKeyEntity> questionKeys = (from q in questionKeyEntities where q.Question.Id == question.Id select q).ToList();
                    //    foreach (QuestionKeyEntity qke in questionKeys)
                    //    {
                    //        if (!foundUserQuestion)
                    //        {
                    //            foreach (UserKeyEntity uke in employeeKeys)
                    //            {
                    //                if (qke.Keyword.Id == uke.Keyword.Id && !foundUserQuestion)
                    //                {
                    //                    employeeQuestions.Add(question);
                    //                    foundUserQuestion = true;
                    //                }
                    //            }
                    //        }
                    //    }
                    //    if (!foundUserQuestion)
                    //    {
                    //        employeeNotFoundQuestions.Add(question);
                    //    }
                    //}

                    ////If there are remaining questions that are open and not found for this employee
                    ////check if other employees are experts in this question. If not add them also to this employee
                    //if (employeeNotFoundQuestions.Count != 0)
                    //{
                    //    List<UserKeyEntity> otherUserKeys = (from u in ctx.UserKeys where u.User.Id != employeeId select u).ToList();
                    //    foreach (QuestionEntity question in employeeNotFoundQuestions)
                    //    {
                    //        bool foundUserQuestion = false;
                    //        List<QuestionKeyEntity> questionKeys = (from q in questionKeyEntities where q.Question.Id == question.Id select q).ToList();
                    //        foreach (QuestionKeyEntity qke in questionKeys)
                    //        {
                    //            if (!foundUserQuestion)
                    //            {
                    //                foreach (UserKeyEntity uke in otherUserKeys)
                    //                {
                    //                    if (qke.Keyword.Id == uke.Keyword.Id && !foundUserQuestion)
                    //                    {
                    //                        foundUserQuestion = true;
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        if (!foundUserQuestion)
                    //        {
                    //            employeeQuestions.Add(question);
                    //        }
                    //    }
                    //}
                    questions = ConvertEntities.AsQuestions(totalEmplyeeList) as List<Question>;
                }
                else
                {
                    throw new NotFoundException("No User entity exists with the specified ID.");
                }
            }

            return questions;
        }

        public User GetAsker(string id)
        {
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                QuestionEntity entity = GetQuestionEntity(Convert.ToInt32(id));
                return entity.User.AsUser();
            }
        }

        public User GetAnswerer(string id)
        {
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                QuestionEntity entity = GetQuestionEntity(Convert.ToInt32(id));
                return entity.Answerer.AsUser();
            }
        }

        public Answer GetAnswer(string id)
        {
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                QuestionEntity entity = GetQuestionEntity(Convert.ToInt32(id));
                return entity.Answer.AsAnswer();
            }
        }

        public IList<Keyword> GetKeywords(string id)
        {
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                QuestionEntity entity = GetQuestionEntity(Convert.ToInt32(id));
                List<KeywordEntity> keywordEntities = ctx.QuestionKeys.Select(x => x.Keyword).ToList();
                List<Keyword> keywords = keywordEntities.Select(x => x.AsKeyword()).ToList();
                return keywords;
            }
        }

        public QuestionEntity GetQuestionEntity(int id)
        {
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                QuestionEntity entity = (from q in ctx.Questions
                                                                 .Include(q => q.Source)
                                                                 .Include(q => q.User)
                                                                 .Include(q => q.User.Sources)
                                                                 .Include(q => q.Answerer)
                                                                 .Include(q => q.Answerer.Sources)
                                                                 .Include(q => q.User.Sources.Select(s => s.SourceDefinition))
                                         where q.Id == id
                                         select q).SingleOrDefault();

                if (entity == null)
                    throw new NotFoundException("No Question entity exists with the specified ID.");

                return entity;
            }
        }
    }
}
