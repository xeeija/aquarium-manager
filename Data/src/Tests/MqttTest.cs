using DAL.Drivers;
using DAL.MongoDB.Entities;
using Services.Drivers;

namespace Tests;

public class MqttTest
{
  [Test]
  public async Task ShouldReadMqtt()
  {
    var mqttDevice = new MQTTDevice()
    {
      Name = "Test Device",
      Host = "localhost",
      DeviceType = DeviceType.Water,
      Active = true,
    };

    var dataPoints = new List<MQTTDataPoint>() {
      new() {
        Name = "Water",
        DataType = DataType.Float,
        Topic = "",
      }
    };

    var driver = new MQTTDriver(mqttDevice, new());

    await driver.Connect();

    await Task.Delay(100);
    Assert.That(driver.IsConnected, Is.True);

    // await driver.Read();
    // Assert.That(driver.Measurements.Count, Is.GreaterThan(0));

    await driver.Disconnect();
  }
}
