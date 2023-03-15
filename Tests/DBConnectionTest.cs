using DAL;

namespace Tests;

public class DBConnectionTest : BaseUnitTests
{
  [Test]
  public void ShouldConnectToDatabase()
  {
    var unit = new UnitOfWork();

    Assert.IsTrue(unit.Context.IsConnected);
  }
}
