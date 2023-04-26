using System.Collections.Concurrent;
using DAL.Influx.Samples;
using DAL.MongoDB.Entities;

namespace DAL.Influx;

public interface IInfluxRepository
{
  Task InsertOneAsync(string bucket, Sample measurement);
  Task InsertManyAsync(string bucket, ConcurrentBag<Sample> measurement);
  Task<List<Sample>> GetInRange(string bucket, DataPoint dp, DateTime from, DateTime to);
  Task<Sample> GetLast(string bucket, DataPoint dp);
  Task CreateBucket(string bucket);
}
