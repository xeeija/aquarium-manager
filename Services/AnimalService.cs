using DAL;
using DAL.Entities;
using DAL.Repository;
using Services.Models.Response;

namespace Services;

public class AnimalService : AquariumItemService<Animal>
{
  public AnimalService(UnitOfWork unit, IRepository<AquariumItem> repository, GlobalService service) : base(unit, (IRepository<Animal>)repository, service)
  {
  }

  public async Task<ItemResponseModel<Animal>> AddAnimal(Animal animal)
  {
    var itemResponse = await AddAquariumItem(animal);

    var response = new ItemResponseModel<Animal>()
    {
      Data = itemResponse.Data as Animal,
      ErrorMessages = itemResponse.ErrorMessages,
    };
    return response;
  }

  public async Task<ItemResponseModel<List<Animal>>> GetAnimals()
  {
    var response = new ItemResponseModel<List<Animal>>()
    {
      Data = unit.AquariumItem.GetAnimals(),
    };
    return response;
  }

  public async Task<bool> Validate(Animal animal)
  {
    await base.Validate(animal);

    if (animal.DeathDate != null)
    {
      modelStateWrapper.AddError("dead animal", "Cannot add a dead animal");
    }

    return modelStateWrapper.IsValid;
  }
}
