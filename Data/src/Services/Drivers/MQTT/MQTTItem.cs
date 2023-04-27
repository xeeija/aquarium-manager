namespace Services.Drivers;

public class MQTTItem
{
  public string Name { get; set; }
  public object Value { get; set; }
  public long Timestamp { get; set; }
}
