using DAL.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Services;

namespace Tests.ServiceTests;

public class CoralServiceTests : BaseUnitOfWorkTests
{
  private CoralService coralService;

  public CoralServiceTests() : base()
  {
    coralService = new CoralService(unit, unit.AquariumItem, null);
  }

  [Test]
  public async Task ShouldAddAnimal()
  {
    var modelState = new Mock<ModelStateDictionary>();
    await coralService.SetModelState(modelState.Object);

    var aquarium = await unit.Aquarium.InsertOneAsync(new Aquarium()
    {
      Liters = 500,
      Height = 55,
      Depth = 65,
      Length = 150,
      WaterType = WaterType.SaltWater,
      Name = "Aquarium Item Test",
    });

    var coral = new Coral()
    {
      Name = "Coralina",
      Amount = 42,
      Species = "Coral",
      CoralType = CoralType.SoftCoral,
      Aquarium = aquarium.ID,
      Description = "coral service test"
    };

    var response = await coralService.AddCoral(coral);

    Assert.NotNull(response);
    Assert.False(response.HasError);
    Assert.NotNull(coral.Inserted);
  }

  public async Task ShouldGetAnimals()
  {
    Task.WaitAll(
      unit.AquariumItem.InsertOneAsync(new Coral()
      {
        Name = "Corason",
        Amount = 1,
        Species = "Coral",
        CoralType = CoralType.SoftCoral,
        Description = "coral service test",
      }),
      unit.AquariumItem.InsertOneAsync(new Coral()
      {
        Name = "Corrie",
        Amount = 2,
        Species = "Coral",
        CoralType = CoralType.HardCoral,
        Description = "coral service test",
      })
    );

    var response = await coralService.GetCorals();
    var animals = response.Data.Where(a => a.Description == "coral service test");

    Assert.NotNull(response);
    Assert.True(response.HasError);
    Assert.AreEqual(animals.Count(), 2);

  }

  [OneTimeTearDown]
  public async Task Teardown()
  {
    var aquariums = new List<string>() {
      "Aquarium Item Test",
    };

    await unit.Aquarium.DeleteManyAsync(aqauarium => aquariums.Contains(aqauarium.Name));
    await unit.AquariumItem.DeleteManyAsync(item => item.Description == "coral service test");
  }
}
