using DAL.Influx;
using DAL.MongoDB.UnitOfWork;

namespace Services
{
  public class GlobalService
  {
    public UnitOfWork UnitOfWork { get; set; }

    // public DeviceService DeviceService { get; set; }
    // public ModbusDeviceService ModbusDeviceService { get; set; }
    // public MQTTDeviceService MQTTDeviceService { get; set; }

    public ValueService ValueService { get; set; }

    public GlobalService(IUnitOfWork UnitOfWork, IInfluxUnitOfWork Influx)
    {
      UnitOfWork uow = (UnitOfWork)UnitOfWork;

      this.UnitOfWork = uow;

      //   DeviceService = new DeviceService(uow, uow.Devices, this);
      //   ModbusDeviceService = new ModbusDeviceService(uow, uow.Devices, this);
      //   MQTTDeviceService = new MQTTDeviceService(uow, uow.Devices, this);
      ValueService = new ValueService(uow, Influx);

    }
  }
}
