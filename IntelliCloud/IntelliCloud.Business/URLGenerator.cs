using System;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;

namespace nl.fhict.IntelliCloud.Business
{
    public class URLGenerator
    {
        public static Uri GenerateResponeURL(QuestionEntity question)
        {
            return new Uri(String.Format("http://81.204.121.229/intellicloud/feedback/index.html?ft={0}", question.FeedbackToken));
        }
    }
}
