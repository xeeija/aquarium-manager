using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DAL.Entities;

[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(typeof(Animal), typeof(Coral))]
// [JsonConverter(typeof(JsonInheritanceConverter), "discriminator")]
[KnownType(typeof(Animal))]
[KnownType(typeof(Coral))]
public abstract class AquariumItem : Entity
{
  public AquariumItem() { }

  public string Aquarium { get; set; }
  public string Name { get; set; }

  public string Species { get; set; }
  public DateTime Inserted { get; set; } = DateTime.Now;

  public int Amount { get; set; }

  public string Description { get; set; }
}
