using nl.fhict.IntelliCloud.Common.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace nl.fhict.IntelliCloud.Business.Plugins.Loader
{
    class MailSendAnswerPlugin : ISendAnswerPlugin
    {
        private SmtpClient client;

        private string clientUsername = "intellicloudquestions@gmail.com";
        private string clientPassword = "proftaaksm72";

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
            try
            {
                client.Send(message);
            }
            catch (Exception e)
            { }
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
                try
                {
                    client = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(clientUsername, clientPassword)
                    };
                }
                catch (Exception e)
                { }
            }
        }
    }
}
