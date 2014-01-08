using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using ActiveUp.Net.Mail;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace IntelliMailClient
{

    /// <summary>
    /// This class contains the service that receives pushmessages from the mailserver
    /// </summary>
    public partial class IntelliMailService : ServiceBase
    {
        Imap4Client client;
        Mailbox inbox;      

        string username = ConfigurationManager.AppSettings["IntelliCloud.Mail.Username"];
        string password = ConfigurationManager.AppSettings["IntelliCloud.Mail.Password"];
        string server = ConfigurationManager.AppSettings["IntelliCloud.Mail.Server"];
        int port = Convert.ToInt32(ConfigurationManager.AppSettings["IntelliCloud.Mail.Port"]);

        /// <summary>
        /// Initializes the class IntelliMailService
        /// </summary>
        public IntelliMailService()
        {
            InitializeComponent();

            //Create an eventlog for the service
            if (!System.Diagnostics.EventLog.SourceExists("IntelliMailSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "IntelliMailSource", "IntelliMailLog");
            }
            //Set the names of the source and the logfile
            serviceLog.Source = "IntelliMailSource";
            serviceLog.Log = "IntelliMailLog";
            serviceLog.WriteEntry("Service initialized", EventLogEntryType.Information);
        }

        /// <summary>
        /// Executed when the service starts
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            serviceLog.WriteEntry("IntelliMailService started");

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(StartIdleProcess);
            if (worker.IsBusy)
                worker.CancelAsync();

            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Executed when the service stops
        /// </summary>
        protected override void OnStop()
        {
            serviceLog.WriteEntry("IntelliMailService stopped");

            //Disconnect client and dispose service
            //client.Disconnect();
            this.Dispose();
        }

        private void StartIdleProcess(object sender, DoWorkEventArgs e)
        {
            ConnectToServer();

            ////Set the messagereceivedeventhandler for the client
            client.NewMessageReceived += new NewMessageReceivedEventHandler(NewMessageReceived);
            serviceLog.WriteEntry("MessageReceivedHandler added");

            SubscribeToInbox();

            client.StartIdle();
        }

        /// <summary>
        /// Creates a connection with the server
        /// </summary>
        private void ConnectToServer()
        {
            //Create a new ImapClient and connect with the server and login with the correct username and password
            client = new Imap4Client();
            client.ConnectSsl(server, port);
            client.Login(username, password);
        }

        /// <summary>
        /// Triggered when the client receives a new e-mail. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void NewMessageReceived(object source, NewMessageReceivedEventArgs e)
        {
            //Write a new entry to the log file.
            serviceLog.WriteEntry("E-mail received!");

            //Stop idling and create a new connection with the server
            client.StopIdle();
            ConnectToServer();

            //Get all unread mails from the inbox
            List<Message> receivedEmails = GetUnreadMails("INBOX").ToList();

            //Send each mail to the IntelliCloud webservice
            foreach (Message mail in receivedEmails) 
            {
                serviceLog.WriteEntry("E-mail received from " + mail.From.Email);
                try
                {
                    CreateQuestion(mail);                    
                    SendConfirmationMail(mail.From);
                }
                catch (Exception ex)
                {
                    //If it fails send error to log file
                    serviceLog.WriteEntry("Sending question failed: " + ex.ToString());
                }
            }

            //Set the messagereceivedeventhandler for the client
            client.NewMessageReceived += new NewMessageReceivedEventHandler(NewMessageReceived);

            SubscribeToInbox();

            client.StartIdle();
        }

        /// <summary>
        /// Creates a question from an e-mail and sends it to backend
        /// </summary>
        /// <param name="mail">The e-mail that contains the question</param>
        private void CreateQuestion(Message mail)
        {
            //Create a new POST request with the correct webmethod
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://81.204.121.229/IntelliCloudService/QuestionService.svc/questions");
            httpWebRequest.ContentType = "application/json; charset=UTF-8";
            httpWebRequest.Method = "POST";

            //Create streamwriter to write data to the webmethod
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                //Serialize the data to json
                QuestionMailObject jsonObject = new QuestionMailObject("Mail", mail.From.Email, mail.BodyText.Text, mail.Subject);
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

        /// <summary>
        /// Subscribe the client to the inbox
        /// </summary>
        private void SubscribeToInbox()
        {
            inbox = client.SelectMailbox("INBOX");
            inbox.Subscribe();
        }

        /// <summary>
        /// Gets all unread mails from the given mailbox
        /// </summary>
        /// <param name="mailBox">Name of the mailbox</param>
        /// <returns></returns>
        private IEnumerable<Message> GetUnreadMails(string mailBox)
        {
            return GetMails(mailBox, "UNSEEN").Cast<Message>();
        }

        /// <summary>
        /// Gets all mails from a given mailbox and searchphrase from the server
        /// </summary>
        /// <param name="mailBox">Name of the mailbox</param>
        /// <param name="searchPhrase">Name of the searchphrase</param>
        /// <returns></returns>
        private MessageCollection GetMails(string mailBox, string searchPhrase)
        {
            Mailbox mails = client.SelectMailbox(mailBox);
            MessageCollection messages = mails.SearchParse(searchPhrase);
            return messages;
        }

    }
}
