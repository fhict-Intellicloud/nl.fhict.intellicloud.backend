using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using nl.fhict.IntelliCloud.Common.DataTransfer;

namespace nl.fhict.IntelliCloud.Data.Model
{
    /// <summary>
    /// A class representing a user.
    /// </summary>
    [Table("User")]
    public class UserEntity
    {
        /// <summary>
        /// Initializes a new instance of <see cref="UserEntity"/>.
        /// </summary>
        public UserEntity()
        {
            this.Sources = new HashSet<SourceEntity>();
        }

        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        [MaxLength(254)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the infix of the users name, e.g. 'van'.
        /// </summary>
        [MaxLength(30)]
        public string Infix { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        [MaxLength(254)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the type of the user. It indicates if a user is an customer or an employee.
        /// </summary>
        public UserType Type { get; set; }
        
        /// <summary>
        /// Gets or sets the creation date and time of the user.
        /// </summary>
        [Required]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets a collection of sources the user supports.
        /// </summary>
        public ICollection<SourceEntity> Sources { get; set; }

    }
}