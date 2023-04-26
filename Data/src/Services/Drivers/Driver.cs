using System.Collections.Concurrent;
using DAL.Influx.Samples;
using DAL.MongoDB.Entities;
using Serilog;
using Utils;

namespace Services.Drivers
{
  public abstract class Driver
  {
    protected ILogger log = Logger.ContextLog<Driver>();

    public ConcurrentDictionary<string, ConcurrentBag<Sample>> Measurements { get; protected set; } = new ConcurrentDictionary<string, ConcurrentBag<Sample>>();
    protected Dictionary<string, DataPoint> DataPoints = new();

    public string Name { get; set; }

    public Driver(string name)
    {
      Name = name;
    }


    public bool IsConnected { get; protected set; }

    public abstract Task Connect();
    public abstract Task Disconnect();


    public void AddNumericMeasurement(string datapoint, NumericSample measurement)
    {
      if (!Measurements.ContainsKey(datapoint))
      {
        Measurements.TryAdd(datapoint, new ConcurrentBag<Sample>());
      }

      Measurements[datapoint].Add(measurement);
    }

    public void AddBinaryMeasurement(string datapoint, BinarySample measurement)
    {
      if (!Measurements.ContainsKey(datapoint))
      {
        Measurements.TryAdd(datapoint, new ConcurrentBag<Sample>());
      }

      Measurements[datapoint].Add(measurement);
    }


    public void AddDataPoint(string name, DataPoint point)
    {
      if (!DataPoints.ContainsKey(name))
      {
        DataPoints.Add(name, point);
      }
    }

    public DataPoint GetDataPoint(string name)
    {
      if (!DataPoints.ContainsKey(name))
      {
        return null;
      }
      else
      {
        return DataPoints[name];
      }
    }

    public async Task Clear()
    {
      Measurements.Clear();
    }
  }
}
