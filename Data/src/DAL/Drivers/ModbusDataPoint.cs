using System.ComponentModel.DataAnnotations;
using DAL.MongoDB.Entities;

namespace DAL.Drivers;

public class ModbusDataPoint : DataPoint
{
  [EnumDataType(typeof(RegisterType))]
  public RegisterType RegisterType { get; set; }

  [EnumDataType(typeof(ReadingType))]
  public ReadingType ReadingType { get; set; }

  public int Register { get; set; }

  public int RegisterCount { get; set; }
}

public enum RegisterType
{
  InputRegister,
  HoldingRegister,
  Coil,
  InputStatus,
}

public enum ReadingType
{
  HighToLow,
  LowToHigh,
}
