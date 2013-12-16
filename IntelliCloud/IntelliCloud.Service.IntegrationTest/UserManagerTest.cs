using Microsoft.VisualStudio.TestTools.UnitTesting;
using nl.fhict.IntelliCloud.Business.Manager;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;
using nl.fhict.IntelliCloud.Data.OpenID.Model;
using System;
using System.Linq;

namespace nl.fhict.IntelliCloud.Service.IntegrationTest
{
    /// <summary>
    /// Test class for the UserManager class.
    /// </summary>
    [TestClass]
    public class UserManagerTest
    {
        #region Fields

        /// <summary>
        /// Instance of class UserManager.
        /// </summary>
        private UserManager manager;

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
            this.manager = new UserManager();
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

                // Add a new Access Token issuer definition
                SourceDefinitionEntity issuerDefinition = new SourceDefinitionEntity() { Name = "accounts.google.com", CreationTime = DateTime.UtcNow };
                context.SourceDefinitions.Add(issuerDefinition);

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
        public void CreateUserTest_UsingOpenIDUserInfo()
        {
            try
            {
                // Create an instance of class OpenIDUserInfo
                OpenIDUserInfo userInfo = new OpenIDUserInfo()
                {
                    Issuer = "accounts.google.com",
                    Sub = "987654321",
                    GivenName = "Name of",
                    FamilyName = "second customer",
                    Name = "Name of second customer",
                    Email = "customer2@domain.com"
                };

                // Create a user using the user info
                this.manager.CreateUser(userInfo);

                // TODO: check if the user object contains the correct data
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        /// <summary>
        /// MatchUser test method.
        /// </summary>
        [TestMethod]
        public void MatchUserTest_UsingOpenIDUserInfo()
        {
            try
            {
                // Create an instance of class OpenIDUserInfo
                OpenIDUserInfo userInfo = new OpenIDUserInfo()
                {
                    Issuer = "accounts.google.com",
                    Sub = "123456789",
                    GivenName = "Name of",
                    FamilyName = "customer",
                    Name = "Name of customer",
                    Email = "customer1@domain.com"
                };

                // Match a user using the user info
                User matchedUser = this.manager.MatchUser(userInfo);

                // TODO: check if the user object contains the correct data
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
