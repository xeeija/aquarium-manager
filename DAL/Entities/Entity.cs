using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DAL.Entities;

public class Entity : IEntity
{
  [BsonId]
  public string ID { get; set; }

  public String GenerateID()
  {
    return ObjectId.GenerateNewId().ToString();
  }
}
