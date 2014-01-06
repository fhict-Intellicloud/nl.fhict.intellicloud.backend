using Microsoft.VisualStudio.TestTools.UnitTesting;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using nl.fhict.IntelliCloud.Common.CustomException;

namespace nl.fhict.IntelliCloud.Service.IntegrationTest
{
    /// <summary>
    /// Test class for the servive UserService.
    /// </summary>
    [TestClass]
    public class UserServiceTest
    {
        #region Fields

        /// <summary>
        /// Instance of class UserService.
        /// </summary>
        private IUserService service;

        /// <summary>
        /// Fields used during testing.
        /// </summary>
        private UserEntity userEntity;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Initialization method for each test method.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.service = new UserService();
            this.initializeTestData();
        }

        /// <summary>
        /// Method that adds test data to the context (used during testing).
        /// </summary>
        private void initializeTestData()
        {
            // First, make sure to remove any existing data
            this.removeTestData();

            // Add test data
            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // Add a new mail definition
                SourceDefinitionEntity mailSourceDefinition = new SourceDefinitionEntity() { Name = "Mail", CreationTime = DateTime.UtcNow };
                context.SourceDefinitions.Add(mailSourceDefinition);

                // Create a new user
                this.userEntity = new UserEntity()
                {
                    FirstName = "Name",
                    Infix = "of",
                    LastName = "first customer",
                    Avatar = "http://domain.com/avatar1.jpg",
                    Type = UserType.Customer,
                    CreationTime = DateTime.UtcNow
                };

                // user to validate the optional after parameters
                UserEntity oldUser = new UserEntity()
                {
                    FirstName = "Name",
                    Infix = "of",
                    LastName = "old employee",
                    Avatar = "http://domain.com/avatar1.jpg",
                    Type = UserType.Customer,
                    CreationTime = new DateTime(2010, 11, 12, 13, 14, 15)
                };
                context.Users.Add(this.userEntity);
                context.Users.Add(oldUser);

                // Create a new source for the user (email address)
                SourceEntity userMailSource = new SourceEntity()
                {
                    Value = "customer1@domain.com",
                    CreationTime = DateTime.UtcNow,
                    SourceDefinition = mailSourceDefinition,
                    User = this.userEntity
                };
                context.Sources.Add(userMailSource);

                // Save the changes to the context
                context.SaveChanges();


            }
        }

        /// <summary>
        /// Cleanup method for each test method.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            this.removeTestData();
        }

        /// <summary>
        /// Method that removes data that was added to the context.
        /// </summary>
        private void removeTestData()
        {
            // Remove all entities from the context
            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                context.Sources.RemoveRange(context.Sources.ToList());
                context.SourceDefinitions.RemoveRange(context.SourceDefinitions.ToList());
                context.Users.RemoveRange(context.Users.ToList());

                // Save the changes to the context
                context.SaveChanges();
            }

            // Unset local variables
            this.userEntity = null;
        }

        #region Tests
        #region GetUserTest
        /// <summary>
        /// GetUser test method that checks if a User is returned when an id has been supplied.
        /// </summary>
        [TestMethod]
        public void GetUserTest()
        {
            try
            {
                // Get the user from the context
                int userId = this.userEntity.Id;
                User user = this.service.GetUser(userId.ToString());

                // Check if the correct data is returned
                Assert.AreEqual(user.FirstName, this.userEntity.FirstName);
                Assert.AreEqual(user.Infix, this.userEntity.Infix);
                Assert.AreEqual(user.LastName, this.userEntity.LastName);
                Assert.AreEqual(user.Type, this.userEntity.Type);
                Assert.AreEqual(user.Avatar, this.userEntity.Avatar);
                user.Sources.Select(s => s.Name.Equals("Mail") && s.Value.Equals("customer1@domain.com"));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        #endregion GetUserTest

        #region Error handling tests

        /// <summary>
        /// Test to validate an Exception is thrown when no user can be found.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NoAnswerFound()
        {
            this.service.GetUser("1000");
        }

        /// <summary>
        /// Test if an ArgumentException is thrown when a negative id is given.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(ArgumentException))]
        public void invalidId_Negative()
        {
            this.service.GetUser("-1");
        }

        /// <summary>
        /// Test if an ArgumentException is thrown when a string that is not a number is given.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(ArgumentException))]
        public void invalidId_NotANumber()
        {
            this.service.GetUser("one");
        }

        /// <summary>
        /// Test if an ArgumentException is thrown when a empty string is given.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(ArgumentException))]
        public void invalidString_EmptyString()
        {
            this.service.AssignKeyword(this.userEntity.Id.ToString(), "", 3);
        }

        /// <summary>
        /// Test if an ArgumentException is thrown when a string with only spaces is given.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(ArgumentException))]
        public void invalidString_spacesString()
        {
            this.service.AssignKeyword(this.userEntity.Id.ToString(), " ", 3);
        }

        /// <summary>
        /// Test if an ArgumentException is thrown when a null value is given as a string.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(ArgumentException))]
        public void invalidString_Null()
        {
            this.service.AssignKeyword(this.userEntity.Id.ToString(), null, 3);
        }

        /// <summary>
        /// Test if an ArgumentException is thrown when a value below 0 is given.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(ArgumentException))]
        public void invalidAffinity_toLow()
        {
            this.service.AssignKeyword(this.userEntity.Id.ToString(), "keyword", -1);
        }

        /// <summary>
        /// Test if an ArgumentException is thrown when a value above 10 is given.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(ArgumentException))]
        public void invalidAffinity_toHigh()
        {
            this.service.AssignKeyword(this.userEntity.Id.ToString(), "keyword", 11);
        }


        /// <summary>
        /// Test if a NotFoundException is thrown when attempting to add keyword to inexisting answer.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(NotFoundException))]
        public void UpdateAnswer_NoAnswerFound()
        {
            this.service.AssignKeyword("1000", "keyword", 5);
        }

        #endregion Error handling tests

        #endregion Tests

        #endregion Methods
    }
}
