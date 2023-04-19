using DAL.MongoDB.Repository.Impl;

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

        public IDataPointRepository DataPoints
        {
            get
            {
                return new DataPointRepository(Context);
            }
        }

        public IDeviceRepository Devices
        {
            get
            {
                return new DeviceRepository(Context);
            }
        }


    }
}
