using System.Linq;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using nl.fhict.IntelliCloud.Business.Plugins.Loader;

namespace nl.fhict.IntelliCloud.Business.Plugins
{
    /// <summary>
    /// A plugin to handle outgoing mails
    /// </summary>
    public class MailSendAnswerPlugin : ISendAnswerPlugin
    {
        private SmtpClient client;

        private readonly string clientUsername = ConfigurationManager.AppSettings["IntelliCloud.Mail.Username"];
        private readonly string clientPassword = ConfigurationManager.AppSettings["IntelliCloud.Mail.Password"];
        private readonly string host = ConfigurationManager.AppSettings["IntelliCloud.Mail.SmtpHost"];
        private readonly int port = Convert.ToInt32(ConfigurationManager.AppSettings["IntelliCloud.Mail.SmtpPort"]);

        /// <summary>
        /// Send an answer through e-mail with the related question
        /// </summary>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        public void SendAnswer(Question question, Answer answer)
        {
            CreateClient();
            
            //Create the from address
            MailAddress fromAddress = new MailAddress(clientUsername, "IntelliCloud Team");

            //Create the to address
            string askerName = question.User.FirstName + " " + question.User.LastName;
            MailAddress toAddress = new MailAddress(question.Source.Source.Value, askerName);

            //Set the e-mail content
            string subject = "Answer to: " + question.Title;
            string body = "Hello " + askerName + ",\n" +
                "\n" +
                "You have recently asked the following question:\n" +
                question.Content + "\n" +
                "\n" +
                "We give you the following answer:\n" +
                answer.Content + "\n" +
                "\n" +
                "We hope to have answered your question.\n" +
                "\n" +
                "Kind regards,\n" +
                "IntelliCloud Team";

            //Create the e-mail with the addresses and content
            using (MailMessage message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })

            //Send the mail
            client.Send(message);
            
            //Dispose the smtp client
            client.Dispose();
        }

        /// <summary>
        /// Creates a new smtp client with Intellicloud credentials
        /// </summary>
        private void CreateClient()
        {
            if (client == null)
            {
                client = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(clientUsername, clientPassword)
                };                
            }
        }
        
        /// <summary>
        /// Send a confirmation mail to the address that send in a question
        /// </summary>
        /// <param name="from">Address that send in a question</param>
        public void SendQuestionRecieved(Question question)
        {
            //Create the from and to addresses that are needed to send the e-mail
            MailAddress fromAddress = new MailAddress(clientUsername, "IntelliCloud Team");
            MailAddress toAddress = new MailAddress(question.User.Sources.Single(s => s.SourceDefinition.Name == "Mail").Value);
            
            //Set the e-mail content
            string subject = "Thank you for your question!";
            string body = "Hello guest,\n\n" + 
                "We received your question. You will soon receive an answer.\n\n" +
                "Kind regards,\n" +
                "IntelliCloud Team";

            //Create a new smtp client with credentials
            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, clientPassword)
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
                    // TODO log real error
                    //serviceLog.WriteEntry("Sending e-mail failed: " + e.ToString());
                }
            //Dispose the smtp client
            smtp.Dispose();
        }
    }
}
