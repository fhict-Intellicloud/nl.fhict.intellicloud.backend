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
using System.Text;
using System.Threading.Tasks;

namespace IntelliTwitterClient.Business.Managers
{
    public class TwitterManager
    {
        private EventLog serviceLog;

        public TwitterManager(EventLog serviceLog)
        {
            this.serviceLog = serviceLog;
        }

        public void StartStreaming()
        {
            //Creates a new TwitterContext with the authorized user 
            using (TwitterContext twitterCtx = new TwitterContext(PinAutharizedUser))
            {
                //Starts a stream which keeps track of the IntelliCloudQ account
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.Filter &&
                       strm.Track == "IntelliCloudQ"
                 select strm)
                .StreamingCallback(strm =>
                {
                    if (strm.Status != TwitterErrorStatus.Success)
                    {
                        //If the stream gives an error status in it's callback we write backend answer in the service log
                        //And we return cause the Content doesn't have to go to the backend
                        serviceLog.WriteEntry(strm.Content);
                        return;
                    }

                    //If the content contains a Question we pass the question the the backend
                    if (!string.IsNullOrWhiteSpace(strm.Content))
                    {
                        var json = JsonMapper.ToObject(strm.Content);
                        var jsonDict = json as IDictionary<string, JsonData>;

                        if (jsonDict.ContainsKey("user") && jsonDict.ContainsKey("text") && jsonDict.ContainsKey("id"))
                        {
                            var scrName = "@" + json["user"]["screen_name"].ToString();
                            var question = json["text"].ToString();
                            var postId = json["id"].ToString();

                            CreateQuestion(scrName, question, postId);
                        }
                    }
                })
                .SingleOrDefault();
            }
        }

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
                        ConsumerKey = "5SFAC0n3LhszMHKvpDkvw",
                        ConsumerSecret = "TkP98l0xDl4FEucVq6WYfEAHyCgJi0b6IwSrOGfhCs",
                        OAuthToken = "2221459926-pUrExE5ls8d0m4D9rIkvSmL7a590XEzKElBOtrr",
                        AccessToken = "2eaC8UZsCdh9E5Pi0JebSZa04VwFFnahkuMf3NVYT41yd"
                    }
                };

                return auth;
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
            Validation.TweetCheck(question);

            //Create a new POST request with the correct webmethod
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://81.204.121.229/IntelliCloudService/QuestionService.svc/questions");
            httpWebRequest.ContentType = "application/json; charset=UTF-8";
            httpWebRequest.Method = "POST";

            //Create streamwriter to write data to the webmethod
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                //Serialize the data to json
                TwitterQuestionObject jsonObject = new TwitterQuestionObject("Twitter", reference, question, question);
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
