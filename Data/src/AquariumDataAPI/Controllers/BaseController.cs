using System.Security.Claims;
using DAL.MongoDB.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Utils;

namespace AquariumDataAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController<T> : ControllerBase where T : Entity
{
  protected DataService<T> Service = null;
  protected Serilog.ILogger log = Logger.ContextLog<BaseController<T>>();
  protected string UserEmail = null;
  protected ClaimsPrincipal ClaimsPrincipal = null;
  public BaseController(DataService<T> service, IHttpContextAccessor accessor)
  {
    Service = service;

    Task model = Service.SetModelState(ModelState);
    model.Wait();
  }

  [HttpGet("{id}")]
  [Authorize]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<T>> Get(string id)
  {
    var result = await Service.Get(id);

    if (result == null)
    {
      return new NotFoundObjectResult(null);
    }

    return result;
  }
}
