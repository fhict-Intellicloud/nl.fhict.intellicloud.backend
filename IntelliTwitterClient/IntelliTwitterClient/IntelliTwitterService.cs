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
    /// <summary>
    /// A windows service to handle incoming tweets
    /// </summary>
    public partial class IntelliTwitterService : ServiceBase
    {
        //The manager holds all the business code for the service
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
            manager.StartStreaming();
        }

        /// <summary>
        /// Disposes the service when it is stopped
        /// </summary>
        protected override void OnStop()
        {
            //Save the last tweet id to app.config
            serviceLog.WriteEntry("IntelliTwitterService stopped");
            this.Dispose();
        }
      
    }
}
