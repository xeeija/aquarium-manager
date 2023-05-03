using DAL.MongoDB.Repository;

namespace DAL.MongoDB.UnitOfWork
{
  public class UnitOfWork : IUnitOfWork
  {
    public DBContext Context { get; private set; } = null;
    public UnitOfWork()
    {
      Context = new DBContext();
    }

    public IDataPointRepository DataPoints => new DataPointRepository(Context);

    public IDeviceRepository Devices => new DeviceRepository(Context);

    public IVisualsRepository Visuals => new VisualsRepository(Context);
  }
}
