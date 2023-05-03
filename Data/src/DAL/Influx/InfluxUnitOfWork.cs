using Context.DataBaseSettings;
using Microsoft.Extensions.Configuration;
using Utils;

namespace DAL.Influx;

public class InfluxUnitOfWork : IInfluxUnitOfWork
{
  private IInfluxRepository _repository = null;

  protected Serilog.ILogger log = Logger.ContextLog<InfluxUnitOfWork>();

  public InfluxDBContext Context { get; private set; } = null;

  public IInfluxRepository Repository => _repository;

  public InfluxUnitOfWork()
  {
    var builder = new ConfigurationBuilder().SetBasePath(Constants.CurrentFolder).AddJsonFile("appsettings.json");

    InfluxDBSettings settings = builder.Build().GetSection("InfluxDbSettings").Get<InfluxDBSettings>();
    var context = new InfluxDBContext(settings);
    Context = context;

    _repository = new InfluxRepository(Context);
  }

}
