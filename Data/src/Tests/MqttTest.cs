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

    var dataPointsSubscribed = new List<MQTTDataPoint>() {
      new() {
        Name = "WaterTemp",
        DataType = DataType.Float,
        Topic = "WaterTemp",
      }
    };

    var driver = new MQTTDriver(mqttDevice, dataPointsSubscribed);

    await driver.Connect();

    await Task.Delay(100);
    Assert.That(driver.IsConnected, Is.True);

    await Task.Delay(4000);

    Assert.That(driver.Measurements, Is.Not.Empty);
    Assert.That(driver.Measurements["WaterTemp"], Is.Not.Empty);

    await driver.Disconnect();
  }
}
