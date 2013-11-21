using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace nl.fhict.IntelliCloud.Common.DataTransfer
{
    /// <summary>
    /// A class representing a user.
    /// </summary>
    [DataContract]
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username of the user. The username is used when a user wishes to be authenticated.
        /// </summary>
        [DataMember]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password of the user. The password is used when a user wishes to be authenticated.
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the infix of the users name, e.g. 'van'.
        /// </summary>
        [DataMember]
        public string Infix { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the type of the user. It indicates if a user is an customer or an employee.
        /// </summary>
        [DataMember]
        public UserType Type { get; set; }

        /// <summary>
        /// Gets or sets a collection of sources the user supports.
        /// </summary>
        [DataMember]
        public IList<Source> Sources { get; set; }

        /// <summary>
        /// Gets or sets the creation date and time of the answer.
        /// </summary>
        [DataMember]
        public DateTime CreationTime { get; set; }
    }
}