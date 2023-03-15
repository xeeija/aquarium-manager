using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAL.Entities;

public class UserAquarium : Entity
{
  public string UserID { get; set; }
  public string AquariumID { get; set; }

  [BsonRepresentation(BsonType.String)]
  public UserRole Role { get; set; }
}

public enum UserRole
{
  User,
  Admin
}
