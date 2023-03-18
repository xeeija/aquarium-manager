using DAL.Entities;

namespace Tests;

public class UserTests : BaseUnitOfWorkTests
{
  [Test]
  public async Task ShouldRegisterAndLoginUser()
  {
    var user = await unit.User.Register(new User()
    {
      FirstName = "Alice",
      LastName = "Lidell",
      Username = "alicebatman",
      Password = "5ecur3P.ssWD",
      IsActive = true,
    });

    Assert.NotNull(user.ID);
    Assert.NotNull(user.HashedPassword);
    Assert.Null(user.Password);

    var loggedInUser = await unit.User.Login("alicebatman", "5ecur3P.ssWD");

    Assert.NotNull(loggedInUser?.ID);
    Assert.AreEqual(loggedInUser?.FirstName, "Alice");
  }

  [Test]
  public async Task ShouldNotLoginInactiveUser()
  {
    var inactiveUser = await unit.User.Register(new User()
    {
      FirstName = "Bob",
      LastName = "Inactive",
      Username = "bobinactive",
      Password = "D1s.abl3d",
      IsActive = false,
    });

    var loggedInUser = await unit.User.Login("bobinactive", "D1s.abl3d");

    Assert.Null(loggedInUser);
  }

  [OneTimeTearDown]
  public async Task Teardown()
  {
    var namesToDelete = new List<string>() {
      "alicebatman",
      "bobinactive",
    };

    await unit.User.DeleteManyAsync(user => namesToDelete.Contains(user.Username));
  }
}
