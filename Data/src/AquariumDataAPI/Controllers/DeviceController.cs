using DAL.Drivers;
using DAL.MongoDB.Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Response.Basis;

namespace AquariumDataAPI.Controllers;

[ProducesResponseType(StatusCodes.Status200OK)]
public class DeviceController : BaseController<Device>
{

  private DeviceService DeviceService;
  private ModbusDeviceService ModbusDeviceService;
  private MqttDeviceService MqttDeviceService;

  public DeviceController(GlobalService service, IHttpContextAccessor accessor) : base(service.DeviceService, accessor)
  {
    DeviceService = service.DeviceService;
    ModbusDeviceService = service.ModbusDeviceService;
    MqttDeviceService = service.MqttDeviceService;
  }

  [HttpGet("GetAllForAquarium/{aquariumId}")]
  public async Task<ActionResult<ItemResponseModel<List<Device>>>> GetLastValues([FromRoute] RouteData route)
  {
    var aquariumId = route.Values["aquariumId"] as string;

    var result = await DeviceService.GetAllForAquarium(aquariumId);
    return result;
  }


  [HttpPost("/Modbus")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<ActionResult<ItemResponseModel<ModbusDevice>>> CreateModbus([FromBody] ModbusDevice device)
  {
    var result = await ModbusDeviceService.AddModbusDevice(device);
    return result;
  }

  [HttpPut("Modbus/{id}")]
  public async Task<ActionResult<ItemResponseModel<ModbusDevice>>> EditModbus([FromRoute] RouteData route, [FromBody] ModbusDevice request)
  {
    var id = route.Values["id"] as string;
    var result = await ModbusDeviceService.Update(id, request);
    return new ItemResponseModel<ModbusDevice>()
    {
      Data = result.Data as ModbusDevice,
      ErrorMessages = result.ErrorMessages,
    };
  }

  [HttpPost("/Mqtt")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<ActionResult<ItemResponseModel<MQTTDevice>>> CreateMqtt([FromBody] MQTTDevice device)
  {
    var result = await MqttDeviceService.AddMqttDevice(device);
    return result;
  }

  [HttpPut("Mqtt/{id}")]
  public async Task<ActionResult<ItemResponseModel<MQTTDevice>>> EditMqtt([FromRoute] RouteData route, [FromBody] MQTTDevice request)
  {
    var id = route.Values["id"] as string;
    var result = await MqttDeviceService.Update(id, request);
    return new ItemResponseModel<MQTTDevice>()
    {
      Data = result.Data as MQTTDevice,
      ErrorMessages = result.ErrorMessages,
    };
  }

}
