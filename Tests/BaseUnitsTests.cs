using DAL;
using Serilog;
using Utils;

namespace Tests;

public class BaseUnitTests
{
  protected ILogger log = Logger.ContextLog<BaseUnitTests>();

  [OneTimeSetUp]
  public async Task Setup()
  {
    Logger.InitLogger();
  }

  [Test]
  public void MyFirstLog()
  {
    log.Information("My first try");
    Assert.IsTrue(true);
  }

  [Test]
  public void ShouldHashPassword()
  {
    var password = "S3cure$Pw.rd";

    var hasher = new Argon2PasswordHasher();
    var passwordHash = hasher.Hash(password);

    Assert.IsTrue(hasher.Verify(password, passwordHash));

  }
}
