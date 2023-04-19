using DAL.MongoDB.Repository.Impl;

namespace DAL.MongoDB.UnitOfWork
{
    public interface IUnitOfWork
    {
        DBContext Context { get; }

        IDataPointRepository DataPoints { get; }

        IDeviceRepository Devices { get; }

    }
}
