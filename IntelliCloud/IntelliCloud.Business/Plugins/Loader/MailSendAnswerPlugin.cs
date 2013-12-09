using nl.fhict.IntelliCloud.Common.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Configuration;

namespace nl.fhict.IntelliCloud.Business.Plugins.Loader
{
    class MailSendAnswerPlugin : ISendAnswerPlugin
    {
        private SmtpClient client;

        private string clientUsername = ConfigurationManager.AppSettings["IntelliCloud.Mail.Username"];
        private string clientPassword = ConfigurationManager.AppSettings["IntelliCloud.Mail.Password"];
        private string host = ConfigurationManager.AppSettings["IntelliCloud.Mail.SmtpHost"];
        private int port = Convert.ToInt32(ConfigurationManager.AppSettings["IntelliCloud.Mail.SmtpPort"]);

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
    }
}
