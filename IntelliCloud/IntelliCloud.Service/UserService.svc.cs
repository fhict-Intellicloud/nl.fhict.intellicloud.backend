using nl.fhict.IntelliCloud.Business.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace nl.fhict.IntelliCloud.Service
{
    public class UserService : IUserService
    {        
        private readonly IntelliCloudManager manager;

        public UserService()
        {
            this.manager = new IntelliCloudManager();            
        }
    }
}
