using DAL.Entities;
using DAL.Repository;

namespace DAL;

public class UnitOfWork : IUnitOfWork
{
  public DBContext Context { get; private set; }

  // public IRepository<Aquarium> Aquarium => new Repository<Aquarium>(Context);
  public IAquarumRepository Aquarium => new AquariumRepository(Context);

  public IAquariumItemRepository AquariumItem => new AquariumItemRepository(Context);

  public IUserRepository User => new UserRepository(Context);

  public UnitOfWork()
  {
    Context = new DBContext();
  }
}
