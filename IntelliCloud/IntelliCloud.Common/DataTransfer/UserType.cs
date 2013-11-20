using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nl.fhict.IntelliCloud.Common.DataTransfer
{
    /// <summary>
    /// An enumeration indicating the type of <see cref="User"/>.
    /// </summary>
    [DataContract]
    public enum UserType
    {
        /// <summary>
        /// Indicates the <see cref="User"/> is an customer.
        /// </summary>
        [EnumMember]
        Customer,
        
        /// <summary>
        /// Indicates the <see cref="User"/> is an employee.
        /// </summary>
        [EnumMember]
        Employee
    }
}
