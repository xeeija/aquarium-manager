using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models.Request;
using Services.Models.Response;

namespace API.Controllers;

[Authorize]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class AquariumController : BaseController<Aquarium>
{
  AquariumService AquariumService { get; set; }
  AnimalService AnimalService { get; set; }
  CoralService CoralService { get; set; }
  PictureService PictureService { get; set; }

  public AquariumController(GlobalService service, IHttpContextAccessor accessor) : base(service.AquariumService, accessor)
  {
    AquariumService = service.AquariumService;
    AnimalService = service.AnimalService;
    CoralService = service.CoralService;
    PictureService = service.PictureService;
  }

  [HttpPost]
  public async Task<ActionResult<ItemResponseModel<Aquarium>>> Create([FromBody] Aquarium aquarium)
  {
    // TODO: Id is required in the endpoint, but not used
    var result = await AquariumService.Create(aquarium);
    return result;
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ItemResponseModel<Aquarium>>> Edit([FromRoute] RouteData route, [FromBody] Aquarium request)
  {
    var id = route.Values["id"] as string;
    return await AquariumService.Update(id, request);
  }

  [HttpGet("ForUser")]
  public async Task<ActionResult<ItemResponseModel<List<Aquarium>>>> ForUser([FromHeader] HeaderDictionary headers)
  {
    // JWT Context?

    var result = await AquariumService.GetForUser("jwt");
    return result;
  }


  // Animal

  [HttpGet("{id}/Animal")]
  public async Task<ActionResult<ItemResponseModel<List<Animal>>>> GetAllAnimals([FromRoute] RouteData route)
  {
    // Does this respect the current aquarium?
    var result = await AnimalService.GetAnimals();
    return result;
  }

  [HttpGet("{id}/Animal/{animalId}")]
  public async Task<ActionResult<Animal>> GetAnimal([FromRoute] RouteData route)
  {
    var id = route.Values["AnimalId"] as string;
    var result = await AnimalService.Get(id);
    return (Animal)result;
  }

  [HttpPost("{id}/Animal")]
  public async Task<ActionResult<ItemResponseModel<Animal>>> CreateAnimal([FromBody] Animal request)
  {
    var result = await AnimalService.AddAnimal(request);
    return result;
  }

  [HttpPut("{id}/Animal/{animalId}")]
  public async Task<ActionResult<ItemResponseModel<Animal>>> EditAnimal([FromRoute] RouteData route, [FromBody] Animal request)
  {
    var id = route.Values["animalId"] as string;
    var result = await AnimalService.Update(id, request);
    return new ItemResponseModel<Animal>()
    {
      Data = (Animal)result.Data,
      ErrorMessages = result.ErrorMessages,
    };
  }


  // Coral

  [HttpGet("{id}/Coral")]
  public async Task<ActionResult<ItemResponseModel<List<Coral>>>> GetAllCorals([FromRoute] RouteData route)
  {
    // Does this respect the current aquarium?
    var result = await CoralService.GetCorals();
    return result;
  }

  [HttpGet("{id}/Coral/{coralId}")]
  public async Task<ActionResult<Coral>> GetCoral([FromRoute] RouteData route)
  {
    var id = route.Values["coralId"] as string;
    var result = await CoralService.Get(id);
    return (Coral)result;
  }

  [HttpPost("{id}/Coral")]
  public async Task<ActionResult<ItemResponseModel<Coral>>> CreateCoral([FromBody] Coral request)
  {
    var result = await CoralService.AddCoral(request);
    return result;
  }

  [HttpPut("{id}/Coral/{coralId}")]
  public async Task<ActionResult<ItemResponseModel<Coral>>> EditCoral([FromRoute] RouteData route, [FromBody] Coral request)
  {
    var id = route.Values["coralId"] as string;
    var result = await CoralService.Update(id, request);
    return new ItemResponseModel<Coral>()
    {
      Data = (Coral)result.Data,
      ErrorMessages = result.ErrorMessages,
    };
  }


  // Picture

  [HttpGet("{id}/Picture/{pictureId}")]
  public async Task<ActionResult<Picture>> GetPicture([FromRoute] RouteData route)
  {
    var id = route.Values["pictureId"] as string;
    var result = await PictureService.Get(id);
    return result;
  }

  [HttpPost("{id}/Picture")]
  public async Task<ActionResult<ItemResponseModel<PictureResponse>>> CreatePicture([FromRoute] RouteData route, [FromBody] PictureRequest request)
  {
    var aquariumId = route.Values["id"] as string;
    var result = await PictureService.AddPicture(aquariumId, request);
    return result;
  }

  [HttpDelete("{id}/Picture/{pictureId}")]
  public async Task<ActionResult<ActionResponseModel>> DeletePicture([FromRoute] RouteData route)
  {
    var id = route.Values["pictureId"] as string;
    var result = await PictureService.Delete(id);
    return result;
  }


}
