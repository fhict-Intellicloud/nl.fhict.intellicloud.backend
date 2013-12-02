using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nl.fhict.IntelliCloud.Data.Context;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    /// <summary>
    /// Other managers will inherit this BaseManager.
    /// This manager has the Validation and ConvertEntities class. Also the IntelliCloudContext for tests.
    /// </summary>
    public abstract class BaseManager
    {
        protected IValidation Validation;
        protected ConvertEntities ConvertEntities;
        protected IntelliCloudContext IntelliCloudContext;

        /// <summary>
        /// This constructor will make the BaseManager and set the given IntelliCloudContext and IValidation.
        /// </summary>
        /// <param name="context">IntelliCloudContext to be set.</param>
        /// <param name="validation">IValidation to be set.</param>
        protected BaseManager(IntelliCloudContext context, IValidation validation)
        {
            Validation = validation;
            ConvertEntities = new ConvertEntities();
            IntelliCloudContext = context;
        }

        /// <summary>
        /// Totally useless summary here, especially made for Teun and Simon.
        /// </summary>
        protected BaseManager()
        {
            Validation = new Validation();
            ConvertEntities = new ConvertEntities();
            IntelliCloudContext = new IntelliCloudContext();
        }
    }
}
