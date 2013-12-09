using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using IntelliTwitterClient.Business.Managers;

namespace IntelliTwitterClient
{
    public partial class IntelliTwitterService : ServiceBase
    {
        //public ulong LastTweetId { get; set; }

        private readonly TwitterManager manager;

        public IntelliTwitterService()
        {
            InitializeComponent();

            this.manager = new TwitterManager(serviceLog);

            bool sourceFound = false;
            try
            {
                sourceFound = EventLog.SourceExists("IntelliTwitterSource");
            }
            catch (SecurityException)
            {
                sourceFound = false;
            }

            //Create an eventlog for the service
            if (!sourceFound)
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "IntelliTwitterSource", "IntelliTwitterLog");
            }

            //Set the names of the source and the logfile
            serviceLog.Source = "IntelliTwitterSource";
            serviceLog.Log = "IntelliTwitterLog";
            serviceLog.WriteEntry("Service initialized", EventLogEntryType.Information);
        }

        /// <summary>
        /// Start the stream to get the tweets
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            serviceLog.WriteEntry("IntelliTwitterService started");
            StartStreaming();
        }

        /// <summary>
        /// Disposes the service when it is stopped
        /// </summary>
        protected override void OnStop()
        {
            serviceLog.WriteEntry("IntelliTwitterService stopped");
            this.Dispose();
        }

        /// <summary>
        /// Starts a twitter api stream it gets all the tweets that are send to the IntelliCloudQ account
        /// It uses the PinAuthorized user to get acces to the stream
        /// </summary>
        private void StartStreaming()
        {
            manager.StartStreaming();
        }

        /// <summary>
        /// Sends a reply to tweet
        /// </summary>
        /// <param name="answer">The answer given by the expert</param>
        /// <param name="reference">The username of the user you want to reply to</param>
        /// <param name="postId">The id of the tweet you want to reply to</param>
        public void SendReplyTweet(string answer, string reference, string postId)
        {
            manager.SendReplyTweet(answer, reference, postId);
        }

        
    }
}
