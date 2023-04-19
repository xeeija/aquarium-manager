using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAL.Entities;

public class Coral : AquariumItem
{
  [BsonRepresentation(BsonType.String)]
  public CoralType CoralType { get; set; }

}

public enum CoralType
{
  HardCoral,
  SoftCoral
}
