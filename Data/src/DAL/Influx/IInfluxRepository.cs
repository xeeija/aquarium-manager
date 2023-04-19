using System.Collections.Concurrent;
using DAL.Influx.Samples;
using Utils;

namespace DAL.Influx;

public interface IInfluxRepository
{
  Task InsertOneAsync(string bucket, Sample measurement);
  Task InsertManyAsync(string bucket, ConcurrentBag<Sample> measurement);
  Task<List<Sample>> GetInRange(string bucket, DataPoint dp, DateTime from, DateTime to);
  Task<Sample> GetLast(string bucket, DataPoint dp);
  Task CreateBucket(string bucket);
}


public class InfluxRepository : IInfluxRepository
{
  protected Serilog.ILogger log = Logger.ContextLog<InfluxRepository>();
  protected InfluxDBContext InfluxDBContext = null;
  string organisation;
  TimeSpan utcOffset;

  // ...

  public async Task InsertOneAsync(string bucket, Sample measurement)
  {
    var point = GeneratePoint(measurement);

    await InfluxDBContext.WriteAPI.WritePointAsync(point, bucket, InfluxDBContext.Organisation);
  }

}
