using Context.DataBaseSettings;
using InfluxDB.Client;
using Utils;

namespace DAL.Influx;

public class InfluxDBContext
{
  Serilog.ILogger log = Logger.ContextLog<InfluxDBContext>();

  public InfluxDBClient DataBase { get; private set; }

  public string Bucket { get; private set; }

  public string Organisation { get; private set; }

  public QueryApi QueryAPI { get; private set; }

  public WriteApiAsync WriteAPI { get; private set; }

  public bool IsConnected
  {
    get
    {
      return DataBase != null;
    }
  }

  public InfluxDBContext(InfluxDBSettings settings)
  {


    String url = "http://" + settings.Server + ":" + settings.Port;

    log.Debug("Connecting to Influx Database: " + url);

    DataBase = new InfluxDBClient(url, settings.Token);

    Bucket = settings.Bucket;
    Organisation = settings.Organization;


    if (DataBase != null)
    {
      log.Information("Successfully connected to Influx DB " + settings.Server + ":" + settings.Port);

      QueryAPI = DataBase.GetQueryApi();

      WriteAPI = DataBase.GetWriteApiAsync();
    }
    else
    {
      log.Fatal("Could not connect to Influx DB " + settings.Server + ":" + settings.Port);
    }

  }
}
