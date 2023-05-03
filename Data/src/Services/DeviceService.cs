using DAL.MongoDB.Entities;
using DAL.MongoDB.Repository;
using DAL.MongoDB.UnitOfWork;
using Services.Response.Basis;

namespace Services;

public class DeviceService : CrudDataService<Device> //where TDevice : Device
{
  public DeviceService(UnitOfWork unit, IRepository<Device> repo, GlobalService service) : base(unit, repo, service)
  {
  }

  public async Task<ItemResponseModel<List<Device>>> GetAllForAquarium(string aquariumId)
  {
    var devices = await UnitOfWork.Devices.FilterByAsync(device => device.Aquarium == aquariumId);

    var response = new ItemResponseModel<List<Device>>()
    {
      Data = devices,
    };
    return response;
  }

  public override async Task<bool> Validate(Device entry)
  {
    return true;
  }

  // public override Task<ItemResponseModel<Device>> Update(string id, Device entry)
  // {

  // }

  // protected override Task<ItemResponseModel<Device>> Create(Device entry)
  // {
  //   throw new NotImplementedException();
  // }
}
