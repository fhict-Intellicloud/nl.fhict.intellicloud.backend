using LinqToTwitter;
using LitJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace IntelliTwitterClient.Business.Managers
{
    /// <summary>
    /// A class that handles all the incoming tweets
    /// </summary>
    public class TwitterManager
    {
        //A log to write messages to, e.g. exceptions
        private EventLog serviceLog { get; set; }
        
        //The accountname of the twitteraccount you want to stream
        private readonly string myScreenName;
        
        /// <summary>
        /// Creates a new PinAutharizedUser
        /// Autorization needed since twitter api 1.1
        /// Account settings can be found in drive - Configuratie settings
        /// </summary>
        private PinAuthorizer PinAutharizedUser
        {
            get
            {
                var auth = new PinAuthorizer
                {
                    Credentials = new InMemoryCredentials
                    {
                        ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"],
                        ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"],
                        OAuthToken = ConfigurationManager.AppSettings["OAuthToken"],
                        AccessToken = ConfigurationManager.AppSettings["AccessToken"]
                    }
                };

                return auth;
            }
        }

        public TwitterManager(EventLog serviceLog)
        {
            this.serviceLog = serviceLog;
            this.myScreenName = ConfigurationManager.AppSettings["ScreenName"];
        }

        /// <summary>
        /// Gets all the tweets since the last tweet that entered the system
        /// This method is used to put tweets in the system that were send when the service wasn't running
        /// </summary>
        /// <param name="postId">Id of the last tweet from the stream</param>
        public void GetTweetSinceId(string postId)
        {
            serviceLog.WriteEntry("GetTweetSinceId SinceID: " + postId);
            ulong sinceId;
            if (ulong.TryParse(postId, out sinceId))
            {
                using (TwitterContext twitterCtx = new TwitterContext(PinAutharizedUser))
                {
                    var userStatusResponse =
                        (from tweet in twitterCtx.Status
                         where tweet.Type == StatusType.Mentions &&
                               tweet.ScreenName == myScreenName &&
                               tweet.SinceID == sinceId
                         select tweet)
                        .ToList();

                    foreach (var tweet in userStatusResponse)
                    {
                        if (!string.IsNullOrWhiteSpace(tweet.Text) && !string.IsNullOrEmpty(tweet.StatusID))
                        {
                            CreateQuestion(tweet.User.Identifier.ScreenName, tweet.Text, tweet.StatusID);
                            SaveLastTweetId(tweet.StatusID);
                        }
                    }
                }
            }
            else
            {
                serviceLog.WriteEntry("Not a valid postId" + sinceId);
            }
        }

        /// <summary>
        /// Saves the id of the last tweet in the app.config file to use when the service has to restart
        /// </summary>
        public void SaveLastTweetId(String lastPostId)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove("LastPostId");
            config.AppSettings.Settings.Add("LastPostId", lastPostId);

            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");

            config.Save(ConfigurationSaveMode.Modified);

        }

        /// <summary>
        /// Starts a twitter api stream it gets all the tweets that are send to the IntelliCloudQ account
        /// It uses the PinAuthorized user to get acces to the stream
        /// </summary>
        public void StartStreaming()
        {

            GetTweetSinceId(ConfigurationManager.AppSettings["LastPostId"]);

            //Creates a new TwitterContext with the authorized user 
            using (TwitterContext twitterCtx = new TwitterContext(PinAutharizedUser))
            {
                //Starts a stream which keeps track of the IntelliCloudQ account
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.Filter &&
                       strm.Track == myScreenName
                 select strm)
                .StreamingCallback(strm =>
                {
                    if (strm.Status != TwitterErrorStatus.Success)
                    {
                        //If the stream gives an error status in it's callback we write the backend answer in the service log
                        //And we return cause the Content doesn't have to go to the backend
                        serviceLog.WriteEntry(strm.Content);
                        return;
                    }

                    //If the content contains a Question we pass the question the the backend
                    if (!string.IsNullOrWhiteSpace(strm.Content))
                    {
                        try
                        {
                            var json = JsonMapper.ToObject(strm.Content);
                            var jsonDict = json as IDictionary<string, JsonData>;

                            if (jsonDict.ContainsKey("user") && jsonDict.ContainsKey("text") && jsonDict.ContainsKey("id"))
                            {
                                var scrName = "@" + json["user"]["screen_name"].ToString();
                                var question = json["text"].ToString();
                                var postId = json["id"].ToString();

                                SaveLastTweetId(postId);
                                CreateQuestion(scrName, question, postId);
                            }
                        }
                        catch (Exception e)
                        {
                            serviceLog.WriteEntry("An exepction occurred during json serialization: " + e.Message);
                        }
                    }
                })
                .SingleOrDefault();
            }
        }

        

        /// <summary>
        /// Creates a new question from an incoming tweet and sends it to the backend
        /// </summary>
        /// <param name="reference">Accountname of the person asking the question</param>
        /// <param name="question">The text of the tweet</param>
        /// <param name="postId">The id of the tweet</param>
        private void CreateQuestion(string reference, string question, string postId)
        {
            Validation.StringCheck(reference);
            Validation.StringCheck(postId);
            Validation.StringCheck(question);

            //Create a new POST request with the correct webmethod
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://81.204.121.229/IntelliCloudServiceNew/QuestionService.svc/questions");
            httpWebRequest.ContentType = "application/json; charset=UTF-8";
            httpWebRequest.Method = "POST";

            //Create streamwriter to write data to the webmethod
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                //Serialize the data to json
                TwitterQuestionObject jsonObject = new TwitterQuestionObject(reference, question, question, postId);
                String json = JsonConvert.SerializeObject(jsonObject);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                //Get the result back and write it into the log file
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    serviceLog.WriteEntry(result);
                }
            }
        }
    }
}
