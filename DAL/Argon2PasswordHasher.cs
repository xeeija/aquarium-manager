using System.Security.Cryptography;
using System.Text;
using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;

namespace DAL;

// see https://github.com/mheyman/Isopoh.Cryptography.Argon2
public class Argon2PasswordHasher : IPasswordHasher
{
  private Argon2Config baseConfig { get; set; }
  private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

  public Argon2PasswordHasher(int timeCost = 10, int memoryCost = 32768, int lanes = 5)
  {
    baseConfig = new Argon2Config
    {
      Type = Argon2Type.HybridAddressing,
      Version = Argon2Version.Nineteen,
      TimeCost = timeCost,
      MemoryCost = memoryCost,
      Lanes = lanes,
      Threads = Environment.ProcessorCount,
    };

  }

  public string Hash(string password)
  {
    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
    byte[] salt = new byte[16];
    Rng.GetBytes(salt);

    var config = new Argon2Config
    {
      Type = baseConfig.Type,
      Version = baseConfig.Version,
      TimeCost = baseConfig.TimeCost,
      MemoryCost = baseConfig.MemoryCost,
      Lanes = baseConfig.Lanes,
      Threads = baseConfig.Threads,
      Password = passwordBytes,
      Salt = salt,
    };

    var argon2 = new Argon2(config);

    using (SecureArray<byte> hash = argon2.Hash())
    {
      return config.EncodeString(hash.Buffer);
    }
  }

  public bool Verify(string password, string passwordHash)
  {
    return Argon2.Verify(passwordHash, password, 4);
  }
}
