using Microsoft.VisualStudio.TestTools.UnitTesting;
using nl.fhict.IntelliCloud.Business.Manager;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;
using nl.fhict.IntelliCloud.Data.OpenID.Model;
using System;
using System.Data.Entity;
using System.Linq;

namespace nl.fhict.IntelliCloud.Service.IntegrationTest
{
    /// <summary>
    /// Test class for the UserManager class.
    /// </summary>
    [TestClass]
    public class UserManagerTest
    {
        //#region Fields

        ///// <summary>
        ///// Instance of class UserManager.
        ///// </summary>
        //private UserManager manager;

        ///// <summary>
        ///// Fields used during testing.
        ///// </summary>
        //private UserEntity userEntity;

        //#endregion Fields

        //#region Methods

        ///// <summary>
        ///// Initialization method for each test method.
        ///// </summary>
        //[TestInitialize]
        //public void Initialize()
        //{
        //    this.manager = new UserManager();
        //    this.initializeTestData();
        //}

        ///// <summary>
        ///// Method that adds test data to the context (used during testing).
        ///// </summary>
        //private void initializeTestData()
        //{
        //    // First, make sure to remove any existing data
        //    this.removeTestData();

        //    // Add test data
        //    using (IntelliCloudContext context = new IntelliCloudContext())
        //    {
        //        // Add a new mail definition
        //        SourceDefinitionEntity mailSourceDefinition = new SourceDefinitionEntity() { Name = "Mail", CreationTime = DateTime.UtcNow };
        //        context.SourceDefinitions.Add(mailSourceDefinition);

        //        // Add a new Access Token issuer definition
        //        SourceDefinitionEntity issuerDefinition = new SourceDefinitionEntity() { Name = "accounts.google.com", CreationTime = DateTime.UtcNow };
        //        context.SourceDefinitions.Add(issuerDefinition);

        //        // Create a new user
        //        this.userEntity = new UserEntity()
        //        {
        //            FirstName = "Name",
        //            Infix = "of",
        //            LastName = "first customer",
        //            Avatar = "http://domain.com/avatar1.jpg",
        //            Type = UserType.Customer,
        //            CreationTime = DateTime.UtcNow
        //        };
        //        context.Users.Add(this.userEntity);

        //        // Create a new source for the user (email address)
        //        SourceEntity userMailSource = new SourceEntity()
        //        {
        //            Value = "customer1@domain.com",
        //            CreationTime = DateTime.UtcNow,
        //            SourceDefinition = mailSourceDefinition,
        //            User = this.userEntity
        //        };
        //        context.Sources.Add(userMailSource);

        //        // Save the changes to the context
        //        context.SaveChanges();
        //    }
        //}

        ///// <summary>
        ///// Cleanup method for each test method.
        ///// </summary>
        //[TestCleanup]
        //public void Cleanup()
        //{
        //    this.removeTestData();
        //}

        ///// <summary>
        ///// Method that removes data that was added to the context.
        ///// </summary>
        //private void removeTestData()
        //{
        //    // Remove all entities from the context
        //    using (IntelliCloudContext context = new IntelliCloudContext())
        //    {
        //        context.Sources.RemoveRange(context.Sources.ToList());
        //        context.SourceDefinitions.RemoveRange(context.SourceDefinitions.ToList());
        //        context.Users.RemoveRange(context.Users.ToList());

        //        // Save the changes to the context
        //        context.SaveChanges();
        //    }

        //    // Unset local variables
        //    this.userEntity = null;
        //}

        //#region Tests

        ///// <summary>
        ///// CreateUser test method.
        ///// </summary>
        //[TestMethod]
        //public void CreateUserTest_UsingOpenIDUserInfo()
        //{
        //    try
        //    {
        //        // Create an instance of class OpenIDUserInfo
        //        OpenIDUserInfo userInfo = new OpenIDUserInfo()
        //        {
        //            Issuer = "accounts.google.com",
        //            Sub = "987654321",
        //            GivenName = "Name of",
        //            FamilyName = "second customer",
        //            Email = "customer2@domain.com",
        //            Picture = "http://domain.com/avatar2.jpg"
        //        };

        //        // Create a user using the user info
        //        this.manager.CreateUser(userInfo);

        //        // Check if the user was added and contains the correct data
        //        using (IntelliCloudContext context = new IntelliCloudContext())
        //        {
        //            UserEntity entity = context.Users
        //                                .Include(u => u.Sources)
        //                                .Include(u => u.Sources.Select(s => s.SourceDefinition))
        //                                .Single(u => u.FirstName.Equals(userInfo.GivenName) && u.LastName.Equals(userInfo.FamilyName));

        //            Assert.AreEqual(entity.Type, UserType.Customer);
        //            entity.Sources.Select(s => s.SourceDefinition.Name.Equals("accounts.google.com") && s.Value.Equals(userInfo.Sub));
        //            entity.Sources.Select(s => s.SourceDefinition.Name.Equals("Mail") && s.Value.Equals(userInfo.Email));
        //            Assert.AreEqual(entity.Avatar, userInfo.Picture);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Assert.Fail(e.Message);
        //    }
        //}

        ///// <summary>
        ///// MatchUser test method.
        ///// </summary>
        //[TestMethod]
        //public void MatchUserTest_UsingOpenIDUserInfo()
        //{
        //    try
        //    {
        //        // Create an instance of class OpenIDUserInfo
        //        OpenIDUserInfo userInfo = new OpenIDUserInfo()
        //        {
        //            Issuer = "accounts.google.com",
        //            Sub = "123456789",
        //            GivenName = "Name",
        //            FamilyName = "first customer",
        //            Email = "customer1@domain.com",
        //            Picture = "http://domain.com/avatar1.jpg"
        //        };

        //        // Match a user using the user info
        //        User user = this.manager.MatchUser(userInfo);

        //        // Check if the user contains the correct data
        //        Assert.AreEqual(user.Type, UserType.Customer);
        //        Assert.AreEqual(user.FirstName, userInfo.GivenName);
        //        Assert.AreEqual(user.LastName, userInfo.FamilyName);
        //        user.Sources.Select(s => s.SourceDefinition.Name.Equals("accounts.google.com") && s.Value.Equals(userInfo.Sub));
        //        user.Sources.Select(s => s.SourceDefinition.Name.Equals("Mail") && s.Value.Equals(userInfo.Email));
        //        Assert.AreEqual(user.Avatar, userInfo.Picture);
        //    }
        //    catch (Exception e)
        //    {
        //        Assert.Fail(e.Message);
        //    }
        //}

        //#endregion Tests

        //#endregion Methods
    }
}
