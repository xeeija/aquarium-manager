using DAL.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Services;

namespace Tests.ServiceTests;

public class AquariumItemServiceTests : BaseUnitOfWorkTests
{
  private AquariumItemService<AquariumItem> aquariumItemService;

  public AquariumItemServiceTests() : base()
  {
    aquariumItemService = new AquariumItemService<AquariumItem>(unit, unit.AquariumItem, null);
  }

  [Test]
  public async Task ShouldAddAquariumItem()
  {
    var modelState = new Mock<ModelStateDictionary>();
    await aquariumItemService.SetModelState(modelState.Object);

    var aquarium = await unit.Aquarium.InsertOneAsync(new Aquarium()
    {
      Liters = 500,
      Height = 55,
      Depth = 65,
      Length = 150,
      WaterType = WaterType.SaltWater,
      Name = "Aquarium Item Test",
    });

    var item = new Animal()
    {
      Name = "Anemonia",
      Amount = 42,
      Species = "Anemone",
      IsAlive = true,
      Aquarium = aquarium.ID,
      Description = "aquariumItem service test"
    };

    var response = await aquariumItemService.AddAquariumItem(item);

    Assert.NotNull(response);
    Assert.False(response.HasError);
    Assert.NotNull(item.Inserted);
  }

  [Test]
  public async Task ShouldFailAddAquariumItemWithoutAquarium()
  {
    var modelState = new Mock<ModelStateDictionary>();
    await aquariumItemService.SetModelState(modelState.Object);

    var item = new Animal()
    {
      Name = "Atlas",
      Amount = 1,
      Species = "Anemone",
      IsAlive = true,
      Description = "aquariumItem service test"
    };

    var response = await aquariumItemService.AddAquariumItem(item);

    Assert.NotNull(response);
    Assert.True(response.HasError);

    item.Aquarium = "something";

    var response2 = await aquariumItemService.AddAquariumItem(item);

    Assert.NotNull(response2);
    Assert.True(response2.HasError);

    var aquarium = await unit.Aquarium.InsertOneAsync(new Aquarium()
    {
      Liters = 500,
      Height = 55,
      Depth = 65,
      Length = 150,
      WaterType = WaterType.SaltWater,
      Name = "Aquarium Item Test",
    });

    item.Aquarium = aquarium.ID;
    item.Amount = 0;

    var response3 = await aquariumItemService.AddAquariumItem(item);

    Assert.NotNull(response3);
    Assert.True(response3.HasError);

  }

  [OneTimeTearDown]
  public async Task Teardown()
  {
    var aquariums = new List<string>() {
      "Aquarium Item Test",
    };

    await unit.Aquarium.DeleteManyAsync(aqauarium => aquariums.Contains(aqauarium.Name));
    await unit.AquariumItem.DeleteManyAsync(item => item.Description == "aquariumItem service test");
  }
}
