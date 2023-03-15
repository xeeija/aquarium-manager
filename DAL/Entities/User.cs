using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DAL.Entities;

public class User : Entity
{
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string FullName { get => $"{FirstName} ${LastName}"; }

  [JsonIgnore]
  [BsonIgnore]
  public int Password { get; set; }
  [JsonIgnore]
  public int HashedPassword { get; set; }

  public bool IsActive { get; set; }
}
