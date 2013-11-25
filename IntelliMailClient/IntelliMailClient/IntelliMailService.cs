using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using ActiveUp.Net.Mail;
using System.Configuration;
using IntelliMailClient.IntelliCloud;
using System.Net.Mail;
using System.Net;

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
        public void NewMessageReceived(object source, NewMessageReceivedEventArgs e)
        {
            //Write a new entry to the log file.
            serviceLog.WriteEntry("E-mail received!");

            IntelliCloudServiceClient intelliCloudClient = new IntelliCloudServiceClient();

            //Stop idling and create a new connection with the server
            client.StopIdle();
            ConnectToServer();

            //Get all unread mails from the inbox
            List<Message> receivedEmails = GetUnreadMails("INBOX").ToList();

            //Send each mail to the IntelliCloud webservice
            foreach (Message m in receivedEmails) 
            {
                serviceLog.WriteEntry("E-mail received from " + m.From.Email);
                try
                {
                    intelliCloudClient.AskQuestion("Mail", m.From.Email, m.BodyText.Text);                    
                    SendConfirmationMail(m.From);
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
        public IEnumerable<Message> GetUnreadMails(string mailBox)
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

        /// <summary>
        /// Send a confirmation mail to the address that send in a question
        /// </summary>
        /// <param name="from">Address that send in a question</param>
        public void SendConfirmationMail(Address from)
        {
            //Create the from and to addresses that are needed to send the e-mail
            MailAddress fromAddress = new MailAddress(username, "IntelliCloud Team");
            MailAddress toAddress = new MailAddress(from.Email, from.Name);
            
            //Set the e-mail content
            string subject = "Thank you for your question!";
            string body = "You will receive an answer in a few days.";

            //Create a new smtp client with credentials
            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, password)
            };

            //Create the e-mail with the addresses and content
            using (MailMessage message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            //Send the mail
                try
                {
                    smtp.Send(message);
                }
                catch (Exception e)
                {
                    //If it fails, the error is written to the logfile
                    serviceLog.WriteEntry("Sending e-mail failed: " + e.ToString());
                }
            //Dispose the smtp client
            smtp.Dispose();
        }
    }
}
