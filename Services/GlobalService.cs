using DAL;

namespace Services;

public class GlobalService
{
  public UserService UserService { get; set; }

  public GlobalService(IUnitOfWork iunit)
  {
    var unit = iunit as UnitOfWork;
    UserService = new UserService(unit, unit.User, null);
  }
}
