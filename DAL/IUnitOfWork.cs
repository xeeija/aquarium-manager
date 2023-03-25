using DAL.Entities;
using DAL.Repository;

namespace DAL;

public interface IUnitOfWork
{
  public DBContext Context { get; }

  public IAquarumRepository Aquarium { get; }

  public IAquariumItemRepository AquariumItem { get; }

  public IUserRepository User { get; }

  public IRepository<UserAquarium> UserAquarium { get; }
}
