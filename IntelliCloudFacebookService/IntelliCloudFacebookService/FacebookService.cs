using Facebook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

namespace IntelliCloudFacebookService
{

    /// <summary>
    /// This is the service class that checks every 30 seconds if there is a new post on the IntelliCloud Facebook page
    /// </summary>
    public partial class IntelliCloudFacebook : ServiceBase
    {
        Timer timer;
        FacebookClient facebookClient;
        string accessToken = ConfigurationManager.AppSettings["IntelliCloud.Facebook.AccesToken"];        


        /// <summary>
        /// Initialize the service
        /// </summary>
        public IntelliCloudFacebook()
        {
            InitializeComponent();

            //Check if the facebooksource exists, if not a new source of the IntelliCloudFacebookLog is created
            if (!System.Diagnostics.EventLog.SourceExists("IntelliCloudFacebookService"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "IntelliCloudFacebookService", "IntelliCloudFacebookLog");
            }
            //Set the source and log for the EventLog
            serviceLog.Source = "IntelliCloudFacebookService";
            serviceLog.Log = "IntelliCloudFacebookLog";
        }

        /// <summary>
        /// Called when service starts
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            //If there is no known TimeOfLastPost, use current time
            if (ConfigurationManager.AppSettings["IntelliCloud.Facebook.TimeOfLastPost"] == "")
            {
                SaveTimeOfLastPost(DateTime.Now.ToString());
            }

            //Start timer which will execute timer_Elapsed every 30 seconds
            timer = new Timer(30000);
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Elapsed);
            timer.Start();
        }

        /// <summary>
        /// Called when the service stops
        /// </summary>
        protected override void OnStop()
        {
        }

        /// <summary>
        /// Called when the timer has elapsed. Checks if the IntelliCloud Facebook page has new posts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Get the time of the post that was last created
            DateTime timeOfLastPost = Convert.ToDateTime(ConfigurationManager.AppSettings["IntelliCloud.Facebook.TimeOfLastPost"]);

            //The whole looking for new posts code is in a try-catch statement. If something goes wrong, for example when there 
            //is no internet connection the service will not crash. It will just try again the next time the timer elapses.
            //The new time of last post is only saved if it succeeds to send the new post to the database.
            try
            {
                //Create a new facebookclient with the accesToken
                facebookClient = new FacebookClient(accessToken);

                //Get the feed from the facebook page bteween the time of the last post and the date of tomorrow
                var wall = (JsonObject)null;
                wall = (JsonObject)facebookClient.Get("Intellicloudquestions/feed?since=" + timeOfLastPost.ToShortDateString() + "&until=" + DateTime.Now.AddDays(1).ToShortDateString());
                dynamic obj = JsonFacebookWall.GetDynamicJsonObject(wall.ToString());

                //Check for each posts if its new
                foreach (var post in obj.data)
                {
                    //First check if the post doesn't come from the page itself
                    if (post.from.name != "Intellicloud")
                    {
                        //Get the time created from the post
                        DateTime timeCreated = (DateTime)post.created_time;

                        //Check if the post was created after the last known post. If true, the post is new
                        if (timeCreated > timeOfLastPost)
                        {
                            //Save question to database
                            CreateQuestion(post);

                            //Set time of last post to the time of the new post
                            SaveTimeOfLastPost(timeCreated.ToString());

                        }
                    }
                }
            }
            //Catch all exceptions
            catch (Exception ex)
            {
                serviceLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Create a new question in the database
        /// </summary>
        /// <param name="post">The dynamic post value</param>
        private void CreateQuestion(dynamic post)
        {
            //Create a new POST request with the correct webmethod
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://81.204.121.229/IntelliCloudService/QuestionService.svc/questions");
            httpWebRequest.ContentType = "application/json; charset=UTF-8";
            httpWebRequest.Method = "POST";

            //Create streamwriter to write data to the webmethod
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                //Get the values from the dynamic post object
                String postFromId = Convert.ToString(post.from.id);
                String postMessage = Convert.ToString(post.message);
                String postId = Convert.ToString(post.id);

                //Serialize the data to json
                JsonFacebookQuestion jsonObject = new JsonFacebookQuestion("Facebook", postFromId, postMessage, "title");
                String json = JsonConvert.SerializeObject(jsonObject);

                //Write the json data
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                //Get the result back and write it into the log file
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Saves the time of the latest post into the app.config file
        /// </summary>
        /// <param name="value">Time created of the post</param>
        private void SaveTimeOfLastPost(String value)
        {
            //Save the new value
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["IntelliCloud.Facebook.TimeOfLastPost"].Value = value;
            config.AppSettings.SectionInformation.ForceSave = true;
            config.Save(ConfigurationSaveMode.Modified);

            //Refresh the settings so the new value can be used
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
