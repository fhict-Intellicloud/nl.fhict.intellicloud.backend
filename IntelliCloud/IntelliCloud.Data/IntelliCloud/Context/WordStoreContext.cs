using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using nl.fhict.IntelliCloud.Data.WordStoreService;

namespace nl.fhict.IntelliCloud.Data.IntelliCloud.Context
{
    /// <summary>
    /// A data context providing acces to wordstore service.
    /// </summary>
    public class WordStoreContext: IDisposable
    {
        /// <summary>
        /// Function to call the WordStore service ResolveWord while ensuring all resources are 
        /// correctly disposed after each call. 
        /// </summary>
        /// <param name="word">The word that needs to be resolved.</param>
        /// <returns>A list with the possibly multiple translations of a given word. 
        /// Null is returned when an error occurs while comminicating.</returns>
        public IList<Word> ResolveWord(string word)
        {
            // Instance of the wordservice client proxy to communicate with the wordstore service.
            WordServiceClient client = new WordServiceClient();

            try
            {
                client.Open();
                var words = client.ResolveWord(word);

                // Whenever a error occurs while communicating with the service the state is set to Faulted.
                if (client.State == CommunicationState.Faulted)
                    client.Abort();
                else
                    client.Close();

                return words;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        /// <summary>
        /// Nothing special needs to be disposed
        /// </summary>
        public void Dispose()
        {
        }
    }
}
