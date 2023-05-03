using DAL.MongoDB.Repository;

namespace DAL.MongoDB.UnitOfWork
{
  public interface IUnitOfWork
  {
    DBContext Context { get; }

    IDataPointRepository DataPoints { get; }

    IDeviceRepository Devices { get; }

    IVisualsRepository Visuals { get; }

  }
}
