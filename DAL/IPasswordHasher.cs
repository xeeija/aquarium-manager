namespace DAL;

public interface IPasswordHasher
{
  string Hash(string password);

  bool Verify(string passwordHash, string storedHash);
}
