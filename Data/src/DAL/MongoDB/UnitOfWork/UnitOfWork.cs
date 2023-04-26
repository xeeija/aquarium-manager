using DAL.MongoDB.Repository;

namespace DAL.MongoDB.UnitOfWork
{
  public class UnitOfWork : IUnitOfWork
  {
    public DBContext Context { get; private set; } = null;
    public UnitOfWork()
    {
      DBContext context = new DBContext();
      Context = context;
    }

    public IDataPointRepository DataPoints => new DataPointRepository(Context);

    // public IDeviceRepository Devices
    // {
    //     get
    //     {
    //         return new DeviceRepository(Context);
    //     }
    // }


  }
}
