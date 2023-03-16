using DAL.Entities;
using DAL.Repository;

namespace DAL;

public class UnitOfWork : IUnitOfWork
{
  public DBContext Context { get; private set; }

  public IRepository<Aquarium> Aquarium => new Repository<Aquarium>(Context);

  public UnitOfWork()
  {
    Context = new DBContext();
  }
}
