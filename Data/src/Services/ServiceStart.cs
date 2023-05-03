using System.Collections.Concurrent;
using DAL.Drivers;
using DAL.Influx;
using DAL.Influx.Samples;
using DAL.MongoDB.Entities;
using DAL.MongoDB.UnitOfWork;
using Services.Drivers;
using Utils;

namespace Services;

public class ServiceStart
{
  protected Serilog.ILogger log = Logger.ContextLog<ServiceStart>();
  IUnitOfWork UnitOfWork = null;
  IInfluxUnitOfWork InfluxUnitOfWork = null;
  Dictionary<String, List<Driver>> Drivers = new Dictionary<String, List<Driver>>();
  System.Timers.Timer timer = null;
  public ServiceStart(IUnitOfWork unitOfWork, IInfluxUnitOfWork InfluxUow)
  {
    UnitOfWork = unitOfWork;
    InfluxUnitOfWork = InfluxUow;
  }

  public async Task Start()
  {
    log.Information("Starting");
    timer = new System.Timers.Timer(10000)
    {
      Enabled = true
    };
    timer.Elapsed += Timer_Elapsed;
    List<MQTTDevice> mqtdevices = UnitOfWork.Devices.GetMQTTDevices();

    foreach (MQTTDevice device in mqtdevices)
    {
      await InfluxUnitOfWork.Repository.CreateBucket(device.Aquarium);
      List<DataPoint> dps = UnitOfWork.DataPoints.GetDataPointsForDevice(device.DeviceType);

      List<MQTTDataPoint> datapoints = dps.Cast<MQTTDataPoint>().ToList();

      var mqtt = new MQTTDriver(device, datapoints);

      if (!Drivers.ContainsKey(device.Aquarium))
      {
        Drivers.Add(device.Aquarium, new List<Driver>());
      }
      Drivers[device.Aquarium].Add(mqtt);

      await Task.Run(() => mqtt.Connect());
    }

    List<ModbusDevice> moddevices = UnitOfWork.Devices.GetModbusDevices();

    foreach (ModbusDevice device in moddevices)
    {
      await InfluxUnitOfWork.Repository.CreateBucket(device.Aquarium);
      List<DataPoint> dps = UnitOfWork.DataPoints.GetDataPointsForDevice(device.DeviceType);

      List<ModbusDataPoint> datapoints = dps.Cast<ModbusDataPoint>().ToList();

      ModbusDriver modbus = new ModbusDriver(device, datapoints);

      if (!Drivers.ContainsKey(device.Aquarium))
      {
        Drivers.Add(device.Aquarium, new List<Driver>());
      }
      Drivers[device.Aquarium].Add(modbus);

      Task.Run(() => modbus.Connect());
    }
    await Save();
  }

  private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
  {
    await Save();
  }

  private async Task Save()
  {
    foreach (KeyValuePair<String, List<Driver>> driver in Drivers)
    {
      var samples = new ConcurrentBag<Sample>();
      foreach (Driver dr in driver.Value)
      {
        foreach (KeyValuePair<String, ConcurrentBag<Sample>> smp in dr.Measurements)
        {
          samples.AddRange(smp.Value);
        }
      }
      await InfluxUnitOfWork.Repository.InsertManyAsync(driver.Key, samples);

      foreach (Driver dr in driver.Value)
      {
        await dr.Clear();
      }
    }
  }

  public async Task Stop()
  {
    foreach (KeyValuePair<String, List<Driver>> driver in Drivers)
    {
      foreach (Driver dr in driver.Value)
      {
        await dr.Disconnect();
      }
    }
  }
}
