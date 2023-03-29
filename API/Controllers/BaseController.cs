using System.Security.Claims;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Utils;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController<T> : ControllerBase where T : Entity
{
  protected Service<T> Service = null;
  protected Serilog.ILogger log = Logger.ContextLog<BaseController<T>>();
  protected string UserEmail = null;
  protected ClaimsPrincipal ClaimsPrincipal = null;
  public BaseController(Service<T> service, IHttpContextAccessor accessor)
  {
    Service = service;

    Task model = Service.SetModelState(ModelState);
    model.Wait();

    if (accessor?.HttpContext?.User == null)
    {
      log.Debug("User is null");
      return;
    }

    ClaimsPrincipal = accessor.HttpContext.User;

    var identity = ClaimsPrincipal.Identity as ClaimsIdentity;

    if (identity == null)
    {
      log.Debug("Identity is null");
      return;
    }

    var claims = identity.Claims;
    var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

    if (emailClaim == null)
    {
      log.Debug("Email is null");
      return;
    }

    UserEmail = emailClaim.Value;
    Task loadUser = Service.Load(UserEmail);
    loadUser.Wait();

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
