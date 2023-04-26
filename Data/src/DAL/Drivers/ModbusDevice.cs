using DAL.MongoDB.Entities;

namespace DAL.Drivers;

public class ModbusDevice : Device
{
  public string Host { get; set; }
  public int Port { get; set; } = 502;

  public int ClientID { get; set; }
}
