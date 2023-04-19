using DAL.Entities;

namespace DAL.Repository;

public class UserRepository : Repository<User>, IUserRepository
{
  IPasswordHasher passwordHasher { get; }

  public UserRepository(DBContext context) : base(context)
  {
    passwordHasher = new Argon2PasswordHasher();
  }

  public async Task<User> Login(string username, string password)
  {
    var user = await FindOneAsync(
      (user) => user.Username == username
      && user.IsActive == true
      // && passwordHasher.Verify(password, user.HashedPassword)
    );
    // return user;

    if (user != null && passwordHasher.Verify(password, user.HashedPassword))
    {
      return user;
    }
    return null;
  }

  public async Task<User> Register(User user)
  {
    user.HashedPassword = passwordHasher.Hash(user.Password);

    await InsertOneAsync(user);
    user.Password = null;

    return user;
  }
}
