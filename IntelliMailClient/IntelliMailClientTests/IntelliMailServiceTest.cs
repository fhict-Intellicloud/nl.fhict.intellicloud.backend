using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ActiveUp.Net.Mail;
using System.Net.Mail;
using System.Net;

namespace IntelliMailClientTests
{
    [TestClass]
    public class IntelliMailServiceTest
    {

        /// <summary>
        /// Testmethod to test the connection with the mail server
        /// </summary>
        [TestMethod]
        public void Connect()
        {
            Imap4Client client;
            string username = "intellicloudquestions@gmail.com";
            string password = "proftaaksm72";
            string server = "imap.gmail.com";
            int port = 993;

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

        [TestMethod]
        public void SendMail()
        {
            var fromAddress = new MailAddress("intellicloudquestions@gmail.com", "IntelliCloud Team");
            var toAddress = new MailAddress("sirojsneksuelk@hotmail.com", "Joris Kleuskens");
            const string fromPassword = "proftaaksm72";
            const string subject = "Thank you for your question!";
            const string body = "You will receive an answer in a few days.";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}
