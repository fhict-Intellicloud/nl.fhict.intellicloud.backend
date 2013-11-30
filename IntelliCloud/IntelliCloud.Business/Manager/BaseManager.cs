using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nl.fhict.IntelliCloud.Data.Context;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    public abstract class BaseManager
    {
        protected IValidation Validation;
        protected ConvertEntities ConvertEntities;
        protected IntelliCloudContext IntelliCloudContext;

        protected BaseManager(IntelliCloudContext context, IValidation validation)
        {
            Validation = validation;
            ConvertEntities = new ConvertEntities();
            IntelliCloudContext = context;
        }

        protected BaseManager()
        {
            Validation = new Validation();
            ConvertEntities = new ConvertEntities();
            IntelliCloudContext = new IntelliCloudContext();
        }
    }
}
