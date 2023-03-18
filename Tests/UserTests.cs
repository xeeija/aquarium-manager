using DAL.Entities;

namespace Tests;

public class UserTests : BaseUnitOfWorkTests
{


  [Test]
  public async Task ShouldLoginUser()
  {

    // TODO: Rreplace with a "register test"
    var hasher = new DAL.Argon2PasswordHasher();

    var user = await unit.User.InsertOneAsync(new User()
    {
      FirstName = "Alice",
      LastName = "Lidell",
      Username = "alicebatman",
      IsActive = true,
      HashedPassword = hasher.Hash("5ecur3P.ssWD"),
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
