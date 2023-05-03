using DAL.Drivers;
using DAL.MongoDB.Entities;
using DAL.MongoDB.UnitOfWork;
using MongoDB.Driver;

namespace DAL.MongoDB.Repository
{
  public class DeviceRepository : Repository<Device>, IDeviceRepository
  {
    public DeviceRepository(DBContext context) : base(context) { }

    public List<MQTTDevice> GetMQTTDevices()
    {
      return Collection.AsQueryable().OfType<MQTTDevice>().ToList();
    }

    public List<ModbusDevice> GetModbusDevices()
    {
      return Collection.AsQueryable().OfType<ModbusDevice>().ToList();
    }
  }
}
