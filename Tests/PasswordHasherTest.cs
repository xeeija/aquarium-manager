using DAL;

namespace Tests;

public class PasswordHasherTest : BaseUnitTests
{
  [Test]
  public void ShouldHashPassword()
  {
    var password = "S3cure$Pw.rd";

    var hasher = new Argon2PasswordHasher();
    var passwordHash = hasher.Hash(password);

    Assert.IsTrue(hasher.Verify(password, passwordHash));

  }
}
