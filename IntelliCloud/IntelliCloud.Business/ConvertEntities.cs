using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Converts an <see cref="AnswerEntity"/> to a <see cref="Answer"/>.
        /// </summary>
        /// <param name="entity">The entity to convert.</param>
        /// <returns>Returns the converted data transfer object.</returns>
        public static Answer AsAnswer(this AnswerEntity entity)
        {
            return new Answer
            {
                Id = string.Format("AnswerService.svc/answers/{0}", entity.Id),
                Content = entity.Content,
                Language = entity.LanguageDefinition.Name,
                User = string.Format("AnswerService.svc/answer/{0}/answerer", entity.Id),
                AnswerState = entity.AnswerState,
                CreationTime = entity.CreationTime,
                LastChangedTime = entity.LastChangedTime,
                Keywords = string.Format("AnswerService.svc/answer/{0}/keywords", entity.Id),
                IsPrivate = entity.IsPrivate,
                Feedbacks = string.Format("AnswerService.svc/answer/{0}/feedbacks", entity.Id),
                Reviews = string.Format("AnswerService.svc/answer/{0}/reviews", entity.Id)
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
                Id = string.Format("FeedbackService.svc/feedbacks/{0}", entity.Id),
                Content = entity.Content,
                Answer = string.Format("FeedbackService.svc/feedbacks/{0}/answer", entity.Id),
                Question = string.Format("FeedbackService.svc/feedbacks/{0}/question", entity.Id),
                User = string.Format("FeedbackService.svc/feedbacks/{0}/user", entity.Id),
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
                Id = string.Format("QuestionService.svc/questions/{0}", entity.Id),
                Title = entity.Title,
                Content = entity.Content,
                Language = entity.LanguageDefinition.Name,
                Answer = string.Format("QuestionService.svc/questions/{0}/answer", entity.Id),
                User = string.Format("QuestionService.svc/questions/{0}/asker", entity.Id),
                Answerer = string.Format("QuestionService.svc/questions/{0}/answerer", entity.Id),
                QuestionState = entity.QuestionState,
                CreationTime = entity.CreationTime,
                LastChangedTime = entity.LastChangedTime,
                Keywords = string.Format("QuestionService.svc/questions/{0}/keywords", entity.Id),
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
                Id = string.Format("ReviewService.svc/reviews/{0}", entity.Id),
                Content = entity.Content,
                Answer = string.Format("ReviewService.svc/reviews/{0}/answer", entity.Id),
                ReviewState = entity.ReviewState,
                User = string.Format("ReviewService.svc/reviews/{0}/user", entity.Id),
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
                Id = string.Format("UserService.svc/users/{0}", entity.Id),
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
                Keywords = string.Format("UserService.svc/users/{0}/keywords", entity.Id),
                Avatar = entity.Avatar == null ? null : entity.Avatar,
                Questions = string.Format("UserService.svc/users/{0}/questions", entity.Id),
                Feedbacks = string.Format("UserService.svc/users/{0}/feedbacks", entity.Id),
                Reviews = string.Format("UserService.svc/users/{0}/reviews", entity.Id),
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
        /// <param name="serviceName">The name of the servicee that uses the keyword.</param>
        /// <returns>returns a converted DTO</returns>
        public static Keyword AsKeyword(this KeywordEntity entity, string serviceName)
        {
            return new Keyword()
            {
                Id = string.Format("{0}/keywords/{1}", serviceName, entity.Id),
                Name = entity.Name,
                CreationTime = entity.CreationTime
            };
        }

        /// <summary>
        /// Converts a <see cref="KeywordEntity"/> collection to a <see cref="Keyword"/> collection.
        /// </summary>
        /// <param name="entities">The entities to convert</param>
        /// <param name="serviceName">The name of the service that uses the keywords.</param>
        /// <returns>Returns the converted DTOs</returns>
        public static IList<Keyword> AsKeywords(this IList<KeywordEntity> entities, string serviceName)
        {
            return entities.Select(k => k.AsKeyword(serviceName)).ToList();
        }

    }
}
