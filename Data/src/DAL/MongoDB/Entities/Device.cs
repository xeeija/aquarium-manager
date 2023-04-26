using System.ComponentModel.DataAnnotations;

namespace DAL.MongoDB.Entities;

public class Device : Entity
{
  public string Name { get; set; }
  public string Description { get; set; }
  public bool Active { get; set; }
  public string Aquarium { get; set; }

  [EnumDataType(typeof(DeviceType))]
  public DeviceType DeviceType { get; set; }
}

public enum DeviceType
{
  Pump,
  Water
}
