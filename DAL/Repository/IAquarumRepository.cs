using DAL.Entities;

namespace DAL.Repository;

public interface IAquarumRepository : IRepository<Aquarium>
{
  Task<Aquarium> GetByName(string name);
}
