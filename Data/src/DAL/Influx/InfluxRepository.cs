using System.Collections.Concurrent;
using DAL.Influx.Samples;
using DAL.MongoDB.Entities;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Writes;
using Utils;

namespace DAL.Influx;

public class InfluxRepository : IInfluxRepository
{

  protected Serilog.ILogger log = Logger.ContextLog<InfluxRepository>();
  protected InfluxDBContext InfluxDBContext = null;
  string organisation;
  TimeSpan utcOffset;

  public InfluxRepository(InfluxDBContext context)
  {
    InfluxDBContext = context;
    organisation = InfluxDBContext.Organisation;
    utcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
  }

  public Task CreateBucket(string bucket)
  {
    throw new NotImplementedException();
  }

  public async Task<List<Sample>> GetInRange(string bucket, DataPoint dp, DateTime from, DateTime to)
  {
    var query = "";
    return await InfluxDBContext.QueryAPI.QueryAsync<Sample>(query, organisation);
  }

  public Task<Sample> GetLast(string bucket, DataPoint dp)
  {
    throw new NotImplementedException();
  }

  public Task InsertManyAsync(string bucket, ConcurrentBag<Sample> measurement)
  {
    throw new NotImplementedException();
  }

  public async Task InsertOneAsync(string bucket, Sample measurement)
  {
    var point = GeneratePoint(measurement);

    await InfluxDBContext.WriteAPI.WritePointAsync(point, bucket, InfluxDBContext.Organisation);
  }

  private PointData GeneratePoint(Sample measurement)
  {
    if (measurement.GetType() == typeof(BinarySample))
    {
      var point = PointData.Measurement(measurement.Tag)
      .Tag("measurement", measurement.Tag)
      .Field("value", measurement.AsBoolean())
      .Timestamp(measurement.Timestamp.ToUniversalTime(), WritePrecision.S);
      return point;
    }
    else
    {
      var point = PointData.Measurement(measurement.Tag)
      .Tag("measurement", measurement.Tag)
      .Field("value", measurement.AsNumeric())
      .Timestamp(measurement.Timestamp.ToUniversalTime(), WritePrecision.S);
      return point;
    }
  }

  private List<Sample> GetSamples(DataPoint dp, List<FluxTable> tables)
  {
    var returnval = new List<Sample>();
    if (dp.DataType == DataType.Boolean)
    {
      foreach (var record in tables.SelectMany(table => table.Records))
      {
        var sample = new BinarySample()
        {
          Timestamp = record.GetTime().Value.ToDateTimeUtc().ToLocalTime().AddHours(utcOffset.Hours),
          Value = Boolean.Parse(record.GetValue().ToString()),
          Tag = record.GetMeasurement()
        };

        returnval.Add(sample);
      }
    }
    else
    {
      foreach (var record in tables.SelectMany(table => table.Records))
      {
        var sample = new NumericSample()
        {
          Timestamp = record.GetTime().Value.ToDateTimeUtc().ToLocalTime().AddHours(utcOffset.Hours),
          Value = float.Parse(record.GetValue().ToString()),
          Tag = record.GetMeasurement()
        };

        returnval.Add(sample);
      }
    }

    return returnval;
  }
}