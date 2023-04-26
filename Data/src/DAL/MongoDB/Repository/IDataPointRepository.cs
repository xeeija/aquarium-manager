using DAL.MongoDB.Entities;

namespace DAL.MongoDB.Repository;

public interface IDataPointRepository : IRepository<DataPoint>
{
  List<DataPoint> GetDataPointsForDevice(DeviceType deviceType);
}
