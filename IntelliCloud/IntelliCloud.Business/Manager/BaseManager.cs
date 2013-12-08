using nl.fhict.IntelliCloud.Data.Context;
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
        protected ConvertEntities ConvertEntities {get; set;}
        protected IntelliCloudContext IntelliCloudContext { get; set; }
        protected IOpenIDContext OpenIDContext { get; set; }

        /// <summary>
        /// This constructor will construct the BaseManager and instantiate it's properties.
        /// The IntelliCloudContext and IValidation properties are set to the given values.
        /// </summary>
        /// <param name="context">IntelliCloudContext to be set.</param>
        /// <param name="validation">IValidation to be set.</param>
        protected BaseManager(IntelliCloudContext context, IValidation validation)
        {
            Validation = validation;
            ConvertEntities = new ConvertEntities();
            IntelliCloudContext = context;
            OpenIDContext = new OpenIDContext();
        }

        /// <summary>
        /// This constructor will construct the BaseManager and instantiate it's properties.
        /// </summary>
        protected BaseManager()
        {
            Validation = new Validation();
            ConvertEntities = new ConvertEntities();
            IntelliCloudContext = new IntelliCloudContext();
            OpenIDContext = new OpenIDContext();
        }
    }
}
