using DAL.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Services;
using Services.Auth;

namespace Tests.ServiceTests;

public class UserServiceTests : BaseUnitOfWorkTests
{
  private UserService userService;

  public UserServiceTests() : base()
  {
    userService = new UserService(unit, unit.User, null);
  }

  [Test]
  public async Task ShouldInsertUser()
  {
    var modelState = new Mock<ModelStateDictionary>();

    await userService.SetModelState(modelState.Object);

    var user = new User()
    {
      FirstName = "Alice",
      LastName = "Lidell",
      Username = "alicebatman",
      Email = "alice@batman.com",
      Password = "5ecur3P.ssWD",
      IsActive = true,
    };

    var userResponse = await userService.CreateHandler(user);

    Assert.NotNull(userResponse);
    Assert.False(userResponse.HasError);
    Assert.AreEqual(user.Email, userResponse.Data?.Email);
  }

  [Test]
  public async Task ShouldFailInsertingInvalidUser()
  {
    var modelState = new Mock<ModelStateDictionary>();
    await userService.SetModelState(modelState.Object);

    var user = new User()
    {
      FirstName = "Alice",
      LastName = "Lidell",
      Username = "alicebatman",
      Password = "5ecur3P.ssWD",
      IsActive = true,
    };

    var userResponse = await userService.CreateHandler(user);

    Assert.NotNull(userResponse);
    Assert.True(userResponse.HasError);
    Assert.Null(userResponse.Data);

    user.Email = "alice@batman.com";

    var userResponse2 = await userService.CreateHandler(user);

    Assert.NotNull(userResponse2);
    Assert.True(userResponse2.HasError);
    Assert.Null(userResponse2.Data);
  }

  [Test]
  public async Task ShouldUpdateUser()
  {
    var modelState = new Mock<ModelStateDictionary>();
    await userService.SetModelState(modelState.Object);

    var user = new User()
    {
      FirstName = "Belice",
      LastName = "Bidell",
      Username = "belicebidell",
      Email = "belice@bidell.com",
      Password = "Secur3P.ssWD",
      IsActive = true,
    };

    var userResponse = await userService.CreateHandler(user);

    Assert.NotNull(userResponse);
    Assert.False(userResponse.HasError);

    userResponse.Data.FirstName = "Celine";
    var updated1 = await userService.UpdateHandler(userResponse.Data.ID, userResponse.Data);

    Assert.NotNull(updated1);
    Assert.False(updated1.HasError);

    updated1.Data.Email = "alice@batman.com";
    var updated2 = await userService.UpdateHandler(updated1.Data.ID, updated1.Data);

    Assert.NotNull(updated2);
    Assert.True(updated2.HasError);

  }

  [Test]
  public async Task ShouldAuthenticateUser()
  {
    var user = await unit.User.Register(new User()
    {
      FirstName = "Peggy",
      LastName = "Black",
      Username = "peggy",
      Email = "peggy@black.com",
      Password = "S3c.rePwd",
      IsActive = true,
    });

    var auth = new Authentication(unit);
    var authInfo = await auth.Authenticate(user);

    Assert.NotNull(authInfo);
    Assert.IsNotEmpty(authInfo.Token);

  }

  [Test]
  public async Task ShouldLoginAndAuthenticateUser()
  {
    var modelState = new Mock<ModelStateDictionary>();
    await userService.SetModelState(modelState.Object);

    var user = await unit.User.Register(new User()
    {
      FirstName = "Peggy",
      LastName = "Black",
      Username = "peggy",
      Email = "peggy@black.com",
      Password = "S3c.rePwd",
      IsActive = true,
    });


    var loginResponse = await userService.Login(new()
    {
      Username = "peggy", //"authelia",
      Password = "S3c.rePwd",
    });

    Assert.NotNull(loginResponse);
    Assert.False(loginResponse.HasError);

    Assert.NotNull(loginResponse.Data?.AuthInfo);
    Assert.IsNotEmpty(loginResponse.Data?.AuthInfo.Token);

  }

  [OneTimeTearDown]
  public async Task Teardown()
  {

    var usersToDelete = new List<string>() {
      "alice@batman.com",
      "belice@bidell.com",
      "peggy@black.com",
    };

    await unit.User.DeleteManyAsync(user => usersToDelete.Contains(user.Email));
  }

}
