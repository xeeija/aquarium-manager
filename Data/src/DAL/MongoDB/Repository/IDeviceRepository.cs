using DAL.Drivers;
using DAL.MongoDB.Entities;

namespace DAL.MongoDB.Repository
{
  public interface IDeviceRepository : IRepository<Device>
  {
    List<MQTTDevice> GetMQTTDevices();
    List<ModbusDevice> GetModbusDevices();
  }
}
