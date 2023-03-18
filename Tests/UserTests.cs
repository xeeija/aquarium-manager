using DAL.Entities;

namespace Tests;

public class UserTests : BaseUnitOfWorkTests
{


  [Test]
  public async Task ShouldLoginUser()
  {

    var user = await unit.User.Register(new User()
    {
      FirstName = "Alice",
      LastName = "Lidell",
      Username = "alicebatman",
      Password = "5ecur3P.ssWD",
      IsActive = true,
    });

    var loggedInUser = await unit.User.Login("alicebatman", "5ecur3P.ssWD");

    Assert.NotNull(loggedInUser.ID);
    Assert.AreEqual(loggedInUser.FirstName, "Alice");
  }

  [TearDown]
  public async Task Teardown()
  {
    var namesToDelete = new List<string>() {
      "alicebatman",
    };

    await unit.User.DeleteManyAsync(user => namesToDelete.Contains(user.Username));
  }
}
