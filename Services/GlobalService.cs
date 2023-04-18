using DAL;

namespace Services;

public class GlobalService
{
  public UserService UserService { get; set; }
  public AquariumService AquariumService { get; set; }
  public AnimalService AnimalService { get; set; }
  public CoralService CoralService { get; set; }
  public PictureService PictureService { get; set; }

  public GlobalService(IUnitOfWork iunit)
  {
    var unit = iunit as UnitOfWork;
    UserService = new UserService(unit, unit.User, null);
    AquariumService = new AquariumService(unit, unit.Aquarium, null);
    AnimalService = new AnimalService(unit, unit.AquariumItem, null);
    CoralService = new CoralService(unit, unit.AquariumItem, null);
    PictureService = new PictureService(unit, unit.Picture, null);
  }
}
