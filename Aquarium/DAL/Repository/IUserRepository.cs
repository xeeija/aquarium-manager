using DAL.Entities;

namespace DAL.Repository;

public interface IUserRepository : IRepository<User>
{
  Task<User> Login(string username, string password);

  Task<User> Register(User user);
}
