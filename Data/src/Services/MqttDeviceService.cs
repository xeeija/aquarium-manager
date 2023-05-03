using DAL.Drivers;
using DAL.MongoDB.Entities;
using DAL.MongoDB.Repository;
using DAL.MongoDB.UnitOfWork;
using Services.Response.Basis;

namespace Services;

public class MqttDeviceService : DeviceService
{
  public MqttDeviceService(UnitOfWork unit, IRepository<Device> repo, GlobalService service) : base(unit, repo, service)
  {
  }

  public async Task<ItemResponseModel<MQTTDevice>> AddMqttDevice(MQTTDevice device)
  {
    var itemResponse = await CreateHandler(device);

    var response = new ItemResponseModel<MQTTDevice>()
    {
      Data = itemResponse.Data as MQTTDevice,
      ErrorMessages = itemResponse.ErrorMessages,
    };
    return response;
  }
}
