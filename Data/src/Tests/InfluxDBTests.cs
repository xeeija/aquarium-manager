using DAL.Influx;
using DAL.Influx.Samples;

namespace Tests;

public class InfluxDBTests
{
  InfluxUnitOfWork unitInflux;

  public InfluxDBTests()
  {
    unitInflux = new InfluxUnitOfWork();
  }

  [Test]
  public async Task ShouldCreateEntry()
  {
    var sample = new NumericSample()
    {
      Tag = "Test",
      Value = 7214,
      Timestamp = DateTime.Now,
    };

    await unitInflux.Repository.InsertOneAsync("test", sample);
  }
}
