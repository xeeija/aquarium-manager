using DAL.Drivers;
using DAL.MongoDB.Entities;
using Services.Drivers;

namespace Tests;

public class ModbusTest
{
  [Test]
  public async Task ShouldReadModbus()
  {
    var modbusDevice = new ModbusDevice()
    {
      Name = "Test Device",
      Host = "localhost",
      ClientID = 1,
      DeviceType = DeviceType.Pump,
      Active = true,
    };

    var dataPoints = new List<ModbusDataPoint>() {
      new() {
        Name = "Current",
        DataType = DataType.Float,
        Register = 0,
        RegisterCount = 2,
        RegisterType = RegisterType.HoldingRegister,
        ReadingType = ReadingType.LowToHigh,
      }
    };

    var driver = new ModbusDriver(modbusDevice, dataPoints);

    await driver.Connect();
    Assert.That(driver.IsConnected, Is.True);

    await driver.Read();
    Assert.That(driver.Measurements.Count, Is.GreaterThan(0));

    await driver.Disconnect();
  }
}
