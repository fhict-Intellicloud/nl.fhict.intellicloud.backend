﻿using System.Linq;
using System.Resources;
using nl.fhict.IntelliCloud.Business.Properties;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using nl.fhict.IntelliCloud.Business.Plugins.Loader;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;

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
        /// Send a confirmation mail to the address that send in a question.
        /// </summary>
        /// <param name="question">Question that was created</param>
        public void SendQuestionReceived(QuestionEntity question)
        {
            //Create the from and to addresses that are needed to send the e-mail
            MailAddress fromAddress = new MailAddress(clientUsername, "IntelliCloud Team");
            MailAddress toAddress = new MailAddress(question.User.Sources.Single(s => s.SourceDefinition.Name == "Mail").Value);
            
            //Set the e-mail content
            ResourceManager rm = Resources.ResourceManager;
            string subject = rm.GetString( question.LanguageDefinition.ResourceName + "_MAIL_AUTO_RESPONSE_SUBJECT");
            string body = String.Format(rm.GetString(question.LanguageDefinition.ResourceName + "_MAIL_AUTO_RESPONSE"),
                question.Content);
            
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
                IsBodyHtml = true,
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

        /// <summary>
        /// Send an answer through e-mail with the related question
        /// </summary>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        public void SendAnswer(QuestionEntity question, AnswerEntity answer)
        {
            CreateClient();

            //Create the from address
            MailAddress fromAddress = new MailAddress(clientUsername, "IntelliCloud Team");

            //Create the to address
            string askerName = question.User.FirstName + " " + question.User.LastName;
            MailAddress toAddress = new MailAddress(question.Source.Source.Value, askerName);

            //Set the e-mail content
            ResourceManager rm = Resources.ResourceManager;
            string subject = rm.GetString(question.LanguageDefinition.ResourceName + "_MAIL_RESPONSE_SUBJECT");
            string body = String.Format(rm.GetString(question.LanguageDefinition.ResourceName + "_MAIL_RESPONSE"),
                question.Content, URLGenerator.GenerateResponeURL(question));
            
            //Create the e-mail with the addresses and content
            using (MailMessage message = new MailMessage(fromAddress, toAddress)
            {
                IsBodyHtml = true,
                Subject = subject,
                Body = body
            })

                //Send the mail
                client.Send(message);

            //Dispose the smtp client
            client.Dispose();
        }
    }
}
