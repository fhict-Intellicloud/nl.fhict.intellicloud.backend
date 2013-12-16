using Microsoft.VisualStudio.TestTools.UnitTesting;
using nl.fhict.IntelliCloud.Business.Manager;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Common.DataTransfer.Input;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

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
        /// TODO: temporarily of class UserManager - should be changed as soon as IUserService is available
        /// </summary>
        private UserManager service;

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
            // TODO: temporarily of class UserManager - should be changed as soon as UserService is available 
            this.service = new UserManager();
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
                    Type = UserType.Customer,
                    CreationTime = DateTime.UtcNow
                };
                context.Users.Add(this.userEntity);

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

        /// <summary>
        /// CreateUser test method.
        /// </summary>
        [TestMethod]
        public void CreateUserTest()
        {
            try
            {
                // Create a new user
                UserType userType = UserType.Customer;
                IList<UserSource> sources = new List<UserSource>();
                sources.Add(new UserSource() { Name = "Mail", Value = "customer2@domain.com" });
                string firstName = "Name";
                string infix = "of";
                string lastName = "second customer";
                string avatar = "http://domain.com/avatar.jpg";
                this.service.CreateUser(userType, sources, firstName, infix, lastName, avatar);

                // Check if the user was added and contains the correct data
                using (IntelliCloudContext context = new IntelliCloudContext())
                {
                    UserEntity entity = context.Users
                                        .Include(u => u.Sources)
                                        .Include(u => u.Sources.Select(s => s.SourceDefinition))
                                        .Single(u => u.FirstName.Equals(firstName) && u.Infix.Equals(infix) && u.LastName.Equals(lastName));

                    Assert.AreEqual(entity.Type, userType);
                    Assert.AreEqual(entity.Avatar, avatar);
                    entity.Sources.Select(s => s.SourceDefinition.Name.Equals(sources[0].Name) && s.Value.Equals(sources[0].Value));
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        /// <summary>
        /// GetUser test method that checks if a User is returned when an id has been supplied.
        /// </summary>
        [TestMethod]
        public void GetUserTest_ById()
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
                user.Sources.Select(s => s.SourceDefinition.Name.Equals("Mail") && s.Value.Equals("customer1@domain.com"));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        /// <summary>
        /// GetUser test method that checks if a User is returned when only a source has been supplied.
        /// </summary>
        [TestMethod]
        public void GetUserTest_BySource()
        {
            try
            {
                // Get the user from the context
                IList<UserSource> sources = new List<UserSource>();
                sources.Add(new UserSource() { Name = "Mail", Value = "customer1@domain.com" });
                User user = this.service.GetUser(null, sources);

                // Check if the correct data is returned
                Assert.AreEqual(user.FirstName, this.userEntity.FirstName);
                Assert.AreEqual(user.Infix, this.userEntity.Infix);
                Assert.AreEqual(user.LastName, this.userEntity.LastName);
                Assert.AreEqual(user.Type, this.userEntity.Type);
                Assert.AreEqual(user.Avatar, this.userEntity.Avatar);
                user.Sources.Select(s => s.SourceDefinition.Name.Equals("Mail") && s.Value.Equals("customer1@domain.com"));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        #endregion Tests

        #endregion Methods
    }
}
