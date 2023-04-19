using DAL.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Services;

namespace Tests.ServiceTests;

public class AnimalServiceTests : BaseUnitOfWorkTests
{
  private AnimalService animalService;

  public AnimalServiceTests() : base()
  {
    animalService = new AnimalService(unit, unit.AquariumItem, null);
  }

  [Test]
  public async Task ShouldAddAnimal()
  {
    var modelState = new Mock<ModelStateDictionary>();
    await animalService.SetModelState(modelState.Object);

    var aquarium = await unit.Aquarium.InsertOneAsync(new Aquarium()
    {
      Liters = 500,
      Height = 55,
      Depth = 65,
      Length = 150,
      WaterType = WaterType.SaltWater,
      Name = "Aquarium Item Test",
    });

    var animal = new Animal()
    {
      Name = "Nemonia",
      Amount = 42,
      Species = "Clownfish",
      IsAlive = true,
      Aquarium = aquarium.ID,
      Description = "animal service test"
    };

    var response = await animalService.AddAnimal(animal);

    Assert.NotNull(response);
    Assert.False(response.HasError);
    Assert.NotNull(animal.Inserted);
  }

  public async Task ShouldGetAnimals()
  {
    Task.WaitAll(
      unit.AquariumItem.InsertOneAsync(new Animal()
      {
        Name = "Nemo",
        IsAlive = true,
        Amount = 1,
        Species = "Clownfish",
        Description = "animal service test",
      }),
      unit.AquariumItem.InsertOneAsync(new Animal()
      {
        Name = "Dorie",
        IsAlive = true,
        Amount = 2,
        Species = "Surgeonfish",
        Description = "animal service test",
      })
    );

    var response = await animalService.GetAnimals();
    var animals = response.Data.Where(a => a.Description == "animal service test");

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
    await unit.AquariumItem.DeleteManyAsync(item => item.Description == "animal service test");
  }
}
