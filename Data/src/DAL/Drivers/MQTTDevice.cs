using DAL.MongoDB.Entities;

namespace DAL.Drivers;

public class MQTTDevice : Device
{
  public string Host { get; set; }
  public int Port { get; set; } = 1883;
}
