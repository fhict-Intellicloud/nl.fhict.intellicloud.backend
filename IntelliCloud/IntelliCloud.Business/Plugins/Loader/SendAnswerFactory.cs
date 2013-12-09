using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nl.fhict.IntelliCloud.Common.DataTransfer;

namespace nl.fhict.IntelliCloud.Business.Plugins.Loader
{
    /// <summary>
    /// A factory that loads the correct send answer plugin according to the given <see cref="SourceDefinition"/>.
    /// A send answer plugin can send an answer using a specific service, like mail, Facebook or Twitter.
    /// </summary>
    internal class SendAnswerFactory
    {
        /// <summary>
        /// Loads the send answer plugin for the given <see cref="SourceDefinition"/>.
        /// </summary>
        /// <param name="sourceDefinition">The source definition for which the plugin needs to be loaded.</param>
        /// <returns>Returns the loaded send answer plugin.</returns>
        public ISendAnswerPlugin LoadPlugin(SourceDefinition sourceDefinition)
        {
            switch (sourceDefinition.Name)
            {
                case "Mail":                   
                    return new MailSendAnswerPlugin();
                case "Facebook":
                    // TODO uncomment
                    // return new FacebookSendAnswerPlugin();
                case "Twitter":
                    // TODO uncomment
                    // return new TwitterSendAnswerPlugin();
                default:
                    throw new NotImplementedException("The provided source is not supported, so the answer couldn't be send.");
            }
        }
    }
}
