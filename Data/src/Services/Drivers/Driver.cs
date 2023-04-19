using DAL.Influx;
using DAL.MongoDB.Entities;
using Serilog;
using System.Collections.Concurrent;
using Utils;

namespace Services.Drivers
{
    public abstract class Driver
    {
        protected ILogger log = Logger.ContextLog<Driver>();

        public ConcurrentDictionary<String, ConcurrentBag<Sample>> Measurements { get; protected set; } = new ConcurrentDictionary<String, ConcurrentBag<Sample>>();
        protected Dictionary<String, DataPoint> DataPoints = new Dictionary<String, DataPoint>();

        public String Name { get; set; }

        public Driver(String name)
        {
            this.Name = name;
        }


        public Boolean IsConnected { get; protected set; }

        public abstract Task Connect();
        public abstract Task Disconnect();


        public void AddNumericMeasurement(String datapoint, NumericSample measurement)
        {
            if (!Measurements.ContainsKey(datapoint))
            {
                Measurements.TryAdd(datapoint, new ConcurrentBag<Sample>());
            }

            Measurements[datapoint].Add(measurement);
        }

        public void AddBinaryMeasurement(String datapoint, BinarySample measurement)
        {
            if (!Measurements.ContainsKey(datapoint))
            {
                Measurements.TryAdd(datapoint, new ConcurrentBag<Sample>());
            }

            Measurements[datapoint].Add(measurement);
        }


        public void AddDataPoint(String name, DataPoint pt)
        {
            if (!DataPoints.ContainsKey(name))
            {
                DataPoints.Add(name, pt);
            }
        }

        public DataPoint GetDataPoint(String name)
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
