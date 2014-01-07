using nl.fhict.IntelliCloud.Business.Plugins.Loader;
using nl.fhict.IntelliCloud.Data.OpenID.Context;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    /// <summary>
    /// Other managers will inherit this BaseManager.
    /// This manager has the Validation and ConvertEntities class. Also the IntelliCloudContext for tests.
    /// </summary>
    public abstract class BaseManager
    {
        protected IValidation Validation {get; set;}
        protected IOpenIDContext OpenIDContext { get; set; }
        protected SendAnswerFactory SendAnswerFactory { get; set; }


        /// <summary>
        /// This constructor will construct the BaseManager and instantiate it's properties.
        /// </summary>
        /// <param name="manager">Manager that serves as the sources for this basemanager</param>
        protected BaseManager(BaseManager manager)
        {
            Validation = manager.Validation;
            OpenIDContext = manager.OpenIDContext;
            SendAnswerFactory = manager.SendAnswerFactory;
        }

        /// <summary>
        /// This constructor will construct the BaseManager and instantiate it's properties.
        /// The IValidation property is set to the given values.
        /// </summary>
        /// <param name="validation">IValidation to be set.</param>
        protected BaseManager(IValidation validation)
        {
            Validation = validation;
            OpenIDContext = new OpenIDContext();
            SendAnswerFactory = new SendAnswerFactory();
        }

        /// <summary>
        /// This constructor will construct the BaseManager and instantiate it's properties.
        /// </summary>
        protected BaseManager()
        {
            Validation = new Validation();
            OpenIDContext = new OpenIDContext();
            SendAnswerFactory = new SendAnswerFactory();
        }
    }
}
