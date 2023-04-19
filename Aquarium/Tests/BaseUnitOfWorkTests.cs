using DAL;

namespace Tests;

public class BaseUnitOfWorkTests : BaseUnitTests
{
  protected UnitOfWork unit;

  public BaseUnitOfWorkTests()
  {
    unit = new UnitOfWork();
  }
}
