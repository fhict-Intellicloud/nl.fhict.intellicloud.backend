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
        /// <param name="questionId">The identifier of the question.</param>
        /// <returns>Returns the question with the given identifier.</returns>
        public Question GetQuestion(string id)
        {
            Validation.IdCheck(id);

            int convertedId = Convert.ToInt32(id);

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
                                         where q.Id == convertedId
                                         select q).SingleOrDefault();

                if (entity == null)
                    throw new NotFoundException("No Question entity exists with the specified ID.");

                question = entity.AsQuestion();
            }
            return question;
        }

        /// <summary>
        /// Retrieves the available questions and filtering them using the state.
        /// </summary>
        /// <param name="state">The optional state of the question, only questions with the given state are returned.</param>
        /// <returns>Returns the questions that match the filter.</returns>
        public IList<Question> GetQuestions(QuestionState? state)
        {
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                var questionEntities = ctx.Questions
                                                                 .Include(q => q.Source)
                                                                 .Include(q => q.User)
                                                                 .Include(q => q.User.Sources)
                                                                 .Include(q => q.Answerer)
                                                                 .Include(q => q.Answerer.Sources)
                                                                 .Include(q => q.QuestionState)
                                                                 .Include(q => q.User.Sources.Select(s => s.SourceDefinition));

                if (state == null)
                    return questionEntities.ToList().AsQuestions();
                else
                {
                    return questionEntities
                        .Where(q => q.QuestionState == state)
                        .ToList()
                        .AsQuestions();

                }
            }
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

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                // TODO determine real language 
                LanguageDefinitionEntity languageDefinition = ctx.LanguageDefinitions.SingleOrDefault(ld => ld.Name.Equals("English"));

                // TODO remove exception as you probably want to create the language if it doesn't exist.
                if (languageDefinition == null)
                    throw new NotFoundException("No languageDefinition entity exists with the specified ID.");

                SourceDefinitionEntity sourceDefinition = ctx.SourceDefinitions.SingleOrDefault(sd => sd.Name.Equals(source));

                if (sourceDefinition == null)
                    throw new NotFoundException("The provided source doesn't exists.");

                // Check if the user already exists
                SourceEntity sourceEntity = ctx.Sources.SingleOrDefault(s => s.SourceDefinition.Id == sourceDefinition.Id && s.Value == reference);

                UserEntity userEntity;

                if (sourceEntity != null)
                {
                    // user already has an account, use this
                    userEntity = ctx.Users.Single(u => u.Id == sourceEntity.UserId);
                }
                else
                {
                    // user has no account, create one
                    userEntity = new UserEntity()
                    {
                        CreationTime = DateTime.UtcNow,
                        Type = UserType.Customer
                    };

                    ctx.Users.Add(userEntity);

                    // Mount the source to the new user
                    sourceEntity = new SourceEntity()
                    {
                        Value = reference,
                        CreationTime = DateTime.UtcNow,
                        SourceDefinition = sourceDefinition,
                        User = userEntity,
                    };

                    ctx.Sources.Add(sourceEntity);
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
                        Source = sourceEntity,
                        PostId = postId
                    },
                    LanguageDefinition = languageDefinition,
                    User = userEntity
                };

                ctx.Questions.Add(questionEntity);

                ctx.SaveChanges();

                // TODO check if there is a 90%+  match
                Boolean match = false;



                // Send auto response
                if (!match)
                {
                    this.SendAnswerFactory.LoadPlugin(questionEntity.Source.Source.SourceDefinition)
                        .SendQuestionReceived(questionEntity);
                }

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
            UserManager userManager = new UserManager(this);

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

                if (userManager.GetUser().Type != UserType.Employee && entity.IsPrivate)
                    throw new NotFoundException("Only employees can retrieve private questions.");

                question = entity.AsQuestion();
            }
            return question;
        }

        /// <summary>
        /// Updates the question with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the question that is updated.</param>
        /// <param name="employeeId">The identifier of the employee that is going to answer the question.</param>
        public void UpdateQuestion(string id, int employeeId)
        {
            // Validate input data
            Validation.IdCheck(id);
            Validation.IdCheck(employeeId);

            // Convert the textual representation of the id to an integer
            int convertedId = Convert.ToInt32(id);

            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // Get the feedback entity from the context
                QuestionEntity question = context.Questions
                                          .SingleOrDefault(q => q.Id == convertedId);

                if (question == null)
                    throw new NotFoundException("No question entity exists with the specified ID.");

                UserEntity user = context.Users
                                         .Include(u => u.Sources)
                                         .Include(u => u.Type)
                                         .SingleOrDefault(u => u.Id == employeeId);

                if (user == null)
                    throw new NotFoundException("No user entity exists with the specified ID.");

                // Update the state of the feedback entry
                question.Answerer = user;
                question.LastChangedTime = DateTime.UtcNow;

                // Save the changes to the context
                context.SaveChanges();
            }
        }

        #region keyword algorith methods

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
                .GroupBy(x => x.Language)
                .Select(x => new { Language = x.Key, Count = x.Distinct().Count() });

            return distinctLanguages.Single(x => x.Count == distinctLanguages.Max(y => y.Count)).Language;
        }
        #endregion

        /// <summary>
        /// Retrieve the user that asked the question with the given identifier.
        /// </summary>
        /// <param name="questionId">The identifier of the question.</param>
        /// <returns>Returns the user that asked the question with the given identifier.</returns>
        public User GetAsker(string id)
        {
            Validation.IdCheck(id);

            int convertedId = Convert.ToInt32(id);

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                return ctx.Questions
                    .Include(q => q.User)
                    .Where(q => q.Id == convertedId)
                    .Select(q => q.User)
                    .Single()
                    .AsUser();
            }
        }

        /// <summary>
        /// Retrieve the user that has answered the question with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the question.</param>
        /// <returns>Returns the user that answered the question with the given identifier.</returns>
        public User GetAnswerer(string id)
        {
            Validation.IdCheck(id);

            int convertedId = Convert.ToInt32(id);
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                return ctx.Questions
                    .Include(q => q.Answerer)
                    .Where(q => q.Id == convertedId)
                    .Select(q => q.Answerer)
                    .Single()
                    .AsUser();
            }
        }

        /// <summary>
        /// Retrieve the answer that answered the question with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the question.</param>
        /// <returns>Returns the answer that answered the question with the given identifier.</returns>
        public Answer GetAnswer(string id)
        {
            Validation.IdCheck(id);

            int convertedId = Convert.ToInt32(id);

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                return ctx.Questions
                    .Include(q => q.Answer)
                    .Where(q => q.Id == convertedId)
                    .Select(q => q.Answer)
                    .Single()
                    .AsAnswer();
            }
        }

        /// <summary>
        /// Retrieve the keywords that are linked to the question with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the question.</param>
        /// <returns>Returns the keywords that are linked to the question with the given identifier.</returns>
        public IList<Keyword> GetKeywords(string id)
        {
            Validation.IdCheck(id);

            int convertedId = Convert.ToInt32(id);

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                return ctx.QuestionKeys
                    .Include(qk => qk.Question)
                     .Include(qk => qk.Keyword)
                    .Where(qk => qk.Question.Id == convertedId)
                    .Select(qk => qk.Keyword)
                    .ToList()
                    .AsKeywords();
            }
        }
    }
}
