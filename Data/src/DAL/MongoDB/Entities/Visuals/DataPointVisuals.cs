using DAL.MongoDB.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace Context.DAL.Visuals
{
  [BsonDiscriminator(RootClass = true)]
  [BsonKnownTypes(typeof(NumericDataPointVisuals), typeof(BinaryDataPointVisuals))]
  public abstract class DataPointVisual : Entity
  {
    public string Name { get; set; }

    public string Description { get; set; }

    public string Icon { get; set; }

  }
}
