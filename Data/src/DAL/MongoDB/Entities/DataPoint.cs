using System.ComponentModel.DataAnnotations;

namespace DAL.MongoDB.Entities;

public class DataPoint : Entity
{
  public string Name { get; set; }
  public string Description { get; set; }
  public string DeviceName { get; set; }

  public int Offset { get; set; } = 1;

  [EnumDataType(typeof(DataType))]
  public DataType DataType { get; set; }

  public string DataPointVisual { get; set; }
}

public enum DataType
{
  Boolean,
  Float,
  Integer
}
