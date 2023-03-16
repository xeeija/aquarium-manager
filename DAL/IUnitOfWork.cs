using DAL.Entities;
using DAL.Repository;

namespace DAL;

public interface IUnitOfWork
{
  public DBContext Context { get; }

  public IRepository<Aquarium> Aquarium { get; }
}
