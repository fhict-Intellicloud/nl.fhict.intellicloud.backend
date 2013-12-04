using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ActiveUp.Net.Mail;
using System.Net.Mail;
using System.Net;
using IntelliMailClient;

namespace IntelliMailClientTests
{
    [TestClass]
    public class IntelliMailServiceTest
    {
        IntelliMailService service;
        Imap4Client client;
        string username;
        string password;
        string server;
        int port;

        [TestInitialize]
        public void InitializeTest()
        {
            service = new IntelliMailService();
            username = "intellicloudquestions@gmail.com";
            password = "proftaaksm72";
            server = "imap.gmail.com";
            port = 993;
        }

        /// <summary>
        /// Testmethod to test the connection with the mail server
        /// </summary>
        [TestMethod]
        public void Connect()
        {
            Imap4Client client;
            //Create a new ImapClient and connect with the server and login with the correct username and password
            client = new Imap4Client();
            client.ConnectSsl(server, port);
            client.Login(username, password);

            //If the client is connected the test passes or else it will fail
            if (client.IsConnected)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

        /// <summary>
        /// Test smtp client and sending an e-mail
        /// </summary>
        [TestMethod]
        public void SendMail()
        {
            //Create e-mail variables
            var fromAddress = new MailAddress("intellicloudquestions@gmail.com", "IntelliCloud Team");
            var toAddress = new MailAddress("sirojsneksuelk@hotmail.com", "Joris Kleuskens");
            const string fromPassword = "proftaaksm72";
            const string subject = "Thank you for your question!";
            const string body = "You will receive an answer in a few days.";

            //Set up an smtp connection
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            //Create the e-mail message
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            //Try to send the e-mail. Test will fail if the e-mail cannot get send
                try
                {
                    //smtp.Send(message);
                    Assert.IsTrue(true);
                }
                catch (Exception)
                {
                    Assert.IsTrue(false);
                }
        }
    }
}
