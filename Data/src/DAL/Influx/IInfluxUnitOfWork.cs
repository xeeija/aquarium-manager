namespace DAL.Influx;

public interface IInfluxUnitOfWork
{
  public IInfluxRepository Repository { get; }
}
