using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;

namespace nl.fhict.IntelliCloud.Business
{
    /// <summary>
    /// A class containing functionality for converting entities to data transfer objects.
    /// </summary>
    public static class ConvertEntities
    {
        /// <summary>
        /// The base URL of the server.
        /// </summary>
        public static Uri baseUrl;

        static ConvertEntities()
        {
            try
            {
                baseUrl = new Uri("http://81.204.121.229/IntelliCloudServiceNew");
            }
            catch (Exception)
            {
                baseUrl = new Uri("http://localhost");
            }
        }

        /// <summary>
        /// Converts an <see cref="AnswerEntity"/> to a <see cref="Answer"/>.
        /// </summary>
        /// <param name="entity">The entity to convert.</param>
        /// <returns>Returns the converted data transfer object.</returns>
        public static Answer AsAnswer(this AnswerEntity entity)
        {
            return new Answer
            {
                Id = new Uri(string.Format("{0}/answers/{1}", baseUrl, entity.Id)),
                Content = entity.Content,
                Language = entity.LanguageDefinition.Name,
                User = new Uri(string.Format("{0}/answer/{1}/answerer", baseUrl, entity.Id)),
                AnswerState = entity.AnswerState,
                CreationTime = entity.CreationTime,
                LastChangedTime = entity.LastChangedTime,
                Keywords = new Uri(string.Format("{0}/answer/{1}/keywords", baseUrl, entity.Id)),
                IsPrivate = entity.IsPrivate,
                Feedbacks = new Uri(string.Format("{0}/answer/{1}/feedbacks", baseUrl, entity.Id)),
                Reviews = new Uri(string.Format("{0}/answer/{1}/reviews", baseUrl, entity.Id))
            };
        }

        /// <summary>
        /// Converts a <see cref="AnswerEntity"/> collection to a <see cref="Answer"/> collection.
        /// </summary>
        /// <param name="entities">The entities to convert.</param>
        /// <returns>Returns the converted data transfer objects.</returns>
        public static IList<Answer> AsAnswers(this IList<AnswerEntity> entities)
        {
            return entities.Select(s => s.AsAnswer()).ToList();
        }

        /// <summary>
        /// Converts an <see cref="FeedbackEntity"/> to a <see cref="Feedback"/>.
        /// </summary>
        /// <param name="entity">The entity to convert.</param>
        /// <returns>Returns the converted data transfer object.</returns>
        public static Feedback AsFeedback(this FeedbackEntity entity)
        {
            return new Feedback
            {
                Id = new Uri(string.Format("{0}/feedbacks/{1}", baseUrl, entity.Id)),
                Content = entity.Content,
                Answer = new Uri(string.Format("{0}/feedbacks/{1}/answer", baseUrl, entity.Id)),
                Question = new Uri(string.Format("{0}/feedbacks/{1}/question", baseUrl, entity.Id)),
                User = new Uri(string.Format("{0}/feedbacks/{1}/user", baseUrl, entity.Id)),
                FeedbackType = entity.FeedbackType,
                FeedbackState = entity.FeedbackState,
                CreationTime = entity.CreationTime,
                LastChangedTime = entity.LastChangedTime
            };
        }

        /// <summary>
        /// Converts a <see cref="FeedbackEntity"/> collection to a <see cref="Feedback"/> collection.
        /// </summary>
        /// <param name="entities">The entities to convert.</param>
        /// <returns>Returns the converted data transfer objects.</returns>
        public static IList<Feedback> AsFeedbacks(this IList<FeedbackEntity> entities)
        {
            return entities.Select(s => s.AsFeedback()).ToList();
        }

        /// <summary>
        /// Converts an <see cref="QuestionEntity"/> to a <see cref="Question"/>.
        /// </summary>
        /// <param name="entity">The entity to convert.</param>
        /// <returns>Returns the converted data transfer object.</returns>
        public static Question AsQuestion(this QuestionEntity entity)
        {
            return new Question
            {
                Id = new Uri(string.Format("{0}/questions/{1}", baseUrl, entity.Id)),
                Title = entity.Title,
                Content = entity.Content,
                Language = entity.LanguageDefinition.Name,
                Answer = new Uri(string.Format("{0}/questions/{1}/answer", baseUrl, entity.Id)),
                User = new Uri(string.Format("{0}/questions/{1}/asker", baseUrl, entity.Id)),
                Answerer = new Uri(string.Format("{0}/questions/{1}/answerer", baseUrl, entity.Id)),
                QuestionState = entity.QuestionState,
                CreationTime = entity.CreationTime,
                LastChangedTime = entity.LastChangedTime,
                Keywords = new Uri(string.Format("{0}/questions/{1}/keywords", baseUrl, entity.Id)),
                IsPrivate = entity.IsPrivate,
                SourcePostId = entity.Source.PostId,
                Source = new UserSource
                {
                    Name = entity.Source.Source.SourceDefinition.Name,
                    Value = entity.Source.Source.Value
                }
            };
        }

        /// <summary>
        /// Converts a <see cref="QuestionEntity"/> collection to a <see cref="Question"/> collection.
        /// </summary>
        /// <param name="entities">The entities to convert.</param>
        /// <returns>Returns the converted data transfer objects.</returns>
        public static IList<Question> AsQuestions(this IList<QuestionEntity> entities)
        {
            return entities.Select(s => s.AsQuestion()).ToList();
        }

        /// <summary>
        /// Converts an <see cref="ReviewEntity"/> to a <see cref="Review"/>.
        /// </summary>
        /// <param name="entity">The entity to convert.</param>
        /// <returns>Returns the converted data transfer object.</returns>
        public static Review AsReview(this ReviewEntity entity)
        {
            return new Review
            {
                Id = new Uri(string.Format("{0}/reviews/{1}", baseUrl, entity.Id)),
                Content = entity.Content,
                Answer = new Uri(string.Format("{0}/reviews/{1}/answer", baseUrl, entity.Id)),
                ReviewState = entity.ReviewState,
                User = new Uri(string.Format("{0}/reviews/{1}/user", baseUrl, entity.Id)),
                CreationTime = entity.CreationTime,
                LastChangedTime = entity.LastChangedTime
            };
        }

        /// <summary>
        /// Converts a <see cref="ReviewEntity"/> collection to a <see cref="Review"/> collection.
        /// </summary>
        /// <param name="entities">The entities to convert.</param>
        /// <returns>Returns the converted data transfer objects.</returns>
        public static IList<Review> AsReviews(this IList<ReviewEntity> entities)
        {
            return entities.Select(s => s.AsReview()).ToList();
        }

        /// <summary>
        /// Converts an <see cref="UserEntity"/> to a <see cref="User"/>.
        /// </summary>
        /// <param name="entity">The entity to convert.</param>
        /// <returns>Returns the converted data transfer object.</returns>
        public static User AsUser(this UserEntity entity)
        {
            return new User
            {
                Id = new Uri(string.Format("{0}/users/{1}", baseUrl, entity.Id)),
                FirstName = entity.FirstName,
                Infix = entity.Infix,
                LastName = entity.LastName,
                Type = entity.Type,
                Sources = entity.Sources.Select(s => new UserSource
                {
                    Name = s.SourceDefinition.Name,
                    Value = s.Value
                }).ToList(),
                CreationTime = entity.CreationTime,
                LastChangedTime = entity.LastChangedTime,
                Keywords = new Uri(string.Format("{0}/users/{1}/keywords", baseUrl, entity.Id)),
                Avatar = entity.Avatar == null ? null : new Uri(entity.Avatar),
                Questions = new Uri(string.Format("{0}/users/{1}/questions", baseUrl, entity.Id)),
                Feedbacks = new Uri(string.Format("{0}/users/{1}/feedbacks", baseUrl, entity.Id)),
                Reviews = new Uri(string.Format("{0}/users/{1}/reviews", baseUrl, entity.Id)),
            };
        }

        /// <summary>
        /// Converts a <see cref="UserEntity"/> collection to a <see cref="User"/> collection.
        /// </summary>
        /// <param name="entities">The entities to convert.</param>
        /// <returns>Returns the converted data transfer objects.</returns>
        public static IList<User> AsUsers(this IList<UserEntity> entities)
        {
            return entities.Select(s => s.AsUser()).ToList();
        }

        /// <summary>
        /// Converts an <see cref="KeywordEntity"/> to a <see cref="Keyword"/>.
        /// </summary>
        /// <param name="entity">The entity to convert</param>
        /// <returns>returns a converted DTO</returns>
        public static Keyword AsKeyword(this KeywordEntity entity)
        {
            return new Keyword()
            {
                Id = new Uri(string.Format("{0}/keywords/{1}", baseUrl, entity.Id)),
                Name = entity.Name,
                CreationTime = entity.CreationTime
            };
        }

        /// <summary>
        /// Converts a <see cref="KeywordEntity"/> collection to a <see cref="Keyword"/> collection.
        /// </summary>
        /// <param name="entities">The entities to convert</param>
        /// <returns>Returns the converted DTOs</returns>
        public static IList<Keyword> AsKeywords(this IList<KeywordEntity> entities)
        {
            return entities.Select(k => k.AsKeyword()).ToList();
        }

    }
}
