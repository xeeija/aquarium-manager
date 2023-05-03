using DAL.Influx.Samples;
using DataCollector.ReturnModels;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace AquariumDataAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValueController : ControllerBase
{
  private ValueService ValueService;

  public ValueController(GlobalService service, IHttpContextAccessor accessor)
  {
    ValueService = service.ValueService;
  }

  [HttpGet("{aquariumId}/GetLastValue/{id}")]
  public async Task<ActionResult<ValueReturnModelSingle>> GetLastValue([FromRoute] RouteData route)
  {
    var id = route.Values["id"] as string;
    var aquariumId = route.Values["aquariumId"] as string;

    var result = await ValueService.GetLastValue(aquariumId, id);
    return result;
  }

  [HttpGet("{aquariumId}/GetLastValues")]
  public async Task<ActionResult<List<ValueReturnModelSingle>>> GetLastValues([FromRoute] RouteData route)
  {
    var aquariumId = route.Values["aquariumId"] as string;

    var result = await ValueService.GetLastValues(aquariumId);
    return result;
  }

}
