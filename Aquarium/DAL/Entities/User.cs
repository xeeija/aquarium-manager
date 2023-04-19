using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DAL.Entities;

public class User : Entity
{
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string FullName => $"{FirstName} {LastName}";

  public string Email { get; set; }

  public string Username { get; set; }

  [JsonIgnore]
  [BsonIgnore]
  public string Password { get; set; }
  [JsonIgnore]
  public string HashedPassword { get; set; }

  public bool IsActive { get; set; }
}
