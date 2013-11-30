using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    public abstract class BaseManager
    {
        public static Validation Validation;
        public static ConvertEntities ConvertEntities;

        protected BaseManager()
        {
            Validation = new Validation();
            ConvertEntities = new ConvertEntities();
        }
    }
}
