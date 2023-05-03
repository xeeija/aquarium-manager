using DAL.Drivers;
using DAL.MongoDB.Entities;
using DAL.MongoDB.Repository;
using DAL.MongoDB.UnitOfWork;
using Services.Response.Basis;

namespace Services;

public class ModbusDeviceService : DeviceService
{
  public ModbusDeviceService(UnitOfWork unit, IRepository<Device> repo, GlobalService service) : base(unit, repo, service)
  {
  }

  public async Task<ItemResponseModel<ModbusDevice>> AddModbusDevice(ModbusDevice device)
  {
    var itemResponse = await CreateHandler(device);

    var response = new ItemResponseModel<ModbusDevice>()
    {
      Data = itemResponse.Data as ModbusDevice,
      ErrorMessages = itemResponse.ErrorMessages,
    };
    return response;
  }
}
