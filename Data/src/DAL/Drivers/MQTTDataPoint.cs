using DAL.MongoDB.Entities;

namespace DAL.Drivers;

public class MQTTDataPoint : DataPoint
{
  public string Topic { get; set; }
}
