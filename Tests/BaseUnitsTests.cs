using Serilog;
using Utils;

namespace Tests;

public class BaseUnitTests
{
  protected ILogger log = Logger.ContextLog<BaseUnitTests>();

  [OneTimeSetUp]
  public virtual async Task Setup()
  {
    Logger.InitLogger();
  }

  // [Test]
  // public void MyFirstLog()
  // {
  //   log.Information("My first try");
  //   Assert.IsTrue(true);
  // }
}
