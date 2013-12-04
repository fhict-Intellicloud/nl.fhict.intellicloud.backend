using System.Runtime.Serialization;

namespace nl.fhict.IntelliCloud.Business.Authorization
{
    /// <summary>
    /// Class that represents user information in the OpenID Connect schema.
    /// </summary>
    [DataContract]
    public class OpenIDUserInfo
    {
        /// <summary>
        /// Gets or sets the identifier/subject of the user.
        /// </summary>
        [DataMember(Name = "sub")]
        public string Sub { get; set; }

        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        [DataMember(Name = "given_name")]
        public string GivenName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        [DataMember(Name = "family_name")]
        public string FamilyName { get; set; }

        /// <summary>
        /// Gets or sets the profile URL of the user.
        /// </summary>
        [DataMember(Name = "profile")]
        public string Profile { get; set; }

        /// <summary>
        /// Gets or sets the picture URL of the user.
        /// </summary>
        [DataMember(Name = "picture")]
        public string Picture { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [DataMember(Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets whether the email address of the user has been verified.
        /// </summary>
        [DataMember(Name = "email_verified")]
        public bool EmailVerified { get; set; }

        /// <summary>
        /// Gets or sets the gender of the user.
        /// </summary>
        [DataMember(Name = "gender")]
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the locale of the user.
        /// </summary>
        [DataMember(Name = "locale")]
        public string Locale { get; set; }
    }
}
