using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAL.Entities;

public class Aquarium : Entity
{
  public string Name { get; set; }

  public double Depth { get; set; }
  public double Length { get; set; }
  public double Height { get; set; }

  [BsonRepresentation(BsonType.String)]
  public WaterType WaterType { get; set; }

  public double Liters { get; set; }
}
