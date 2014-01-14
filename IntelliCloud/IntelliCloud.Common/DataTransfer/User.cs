using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace nl.fhict.IntelliCloud.Common.DataTransfer
{
    /// <summary>
    /// A class representing a user.
    /// </summary>
    [DataContract]
    public class User
    {
        /// <summary>
        /// Gets or sets the URL to this specific user.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

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
        public UserType Type { get; set; }

        /// <summary>
        /// Gets or sets a collection of sources the user supports.
        /// </summary>
        [DataMember]
        public IList<UserSource> Sources { get; set; }

        /// <summary>
        /// Gets or sets the creation date and time of the answer.
        /// </summary>
        [DataMember]
        public DateTime? CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the last changed date and time of the user.
        /// </summary>
        [DataMember]
        public DateTime? LastChangedTime { get; set; }

        /// <summary>
        /// Gets or sets the URL to the keywords that are linked to the user.
        /// </summary>
        [DataMember]
        public string Keywords { get; set; }

        /// <summary>
        /// Gets or sets the URL to the avatar image of the user.
        /// </summary>
        [DataMember]
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets the URL to the questions that can be answered by the user. These questions have keywords in 
        /// common with this user.
        /// </summary>
        [DataMember]
        public string Questions { get; set; }

        /// <summary>
        /// Gets or sets the URL to the answers that have open feedback that can be processed by the user. These answers
        /// have keywords in common with this user.
        /// </summary>
        [DataMember]
        public string Feedbacks { get; set; }

        /// <summary>
        /// Gets or sets the URL to the answers that have open reviews that can be processed by the user. These answers
        /// have keywords in common with this user.
        /// </summary>
        [DataMember]
        public string Reviews { get; set; }
    }
}