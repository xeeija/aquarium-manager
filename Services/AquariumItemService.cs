using DAL;
using DAL.Entities;
using DAL.Repository;
using Services.Models.Response;

namespace Services;

public class AquariumItemService : CrudService<AquariumItem>
{
  public AquariumItemService(UnitOfWork unit, IRepository<AquariumItem> repository, GlobalService service) : base(unit, repository, service)
  {
  }

  public Task<ItemResponseModel<AquariumItem>> AddAquariumItem(AquariumItem item)
  {
    return CreateHandler(item);
  }

  public override async Task<bool> Validate(AquariumItem item)
  {
    if (item == null)
    {
      modelStateWrapper.AddError("AquariumItem null", "Aquarium item is null");
      return modelStateWrapper.IsValid;
    }

    var aquarium = await unit.Aquarium.FindByIdAsync(item.Aquarium);
    if (aquarium == null)
    {
      modelStateWrapper.AddError("Aquarium unknown", "Item does not belong to a valid aquarium");
    }
    if (item.Amount == null || item.Amount <= 0)
    {
      modelStateWrapper.AddError("Amount not positive", "Amount must be positive");
    }

    return modelStateWrapper.IsValid;
  }
}
