using DAL.Entities;

namespace Tests.DBTests;

public class AquariumTests : BaseUnitOfWorkTests
{

  private List<Aquarium> createdAquariums = new List<Aquarium>();


  [Test]
  public async Task ShouldCreateAquarium()
  {
    var aquarium = new Aquarium()
    {
      Liters = 500,
      Height = 55,
      Depth = 65,
      Length = 150,
      WaterType = WaterType.SaltWater,
      Name = "Create Aquarium",
    };

    // var unit = new UnitOfWork();

    var createdAquarium = await unit.Aquarium.InsertOneAsync(aquarium);

    Assert.NotNull(createdAquarium);
    Assert.NotNull(createdAquarium.ID);
  }

  public async Task ShouldGetAquariumByName()
  {
    var aquarium = await unit.Aquarium.GetByName("Create Aquarium");

    Assert.NotNull(aquarium.ID);
    Assert.AreEqual(aquarium.Name, "Create Aquarium");
    Assert.AreEqual(aquarium.Length, 500, Double.Epsilon);
    Assert.AreEqual(aquarium.WaterType, WaterType.SaltWater);
  }

  [Test]
  public async Task ShouldUpdateAquarium()
  {
    var aquarium = await unit.Aquarium.InsertOneAsync(new Aquarium()
    {
      Name = "Update Aquarium",
      Depth = 50,
      Length = 60,
      Height = 40,
    });

    aquarium.Height = 42;

    var updatedAquarium = await unit.Aquarium.UpdateOneAsync(aquarium);

    // log.Information(updatedAquarium?.ID);

    // log.Information("updated2");
    // log.Information(updatedAquarium.ToJson());

    Assert.AreEqual(updatedAquarium.Height, 42, Double.Epsilon);
  }

  [Test]
  public async Task ShouldDeleteAquarium()
  {
    var aquarium = await unit.Aquarium.InsertOneAsync(new Aquarium() { Name = "Delete Aquarium" });
    Assert.NotNull(aquarium.ID);

    await unit.Aquarium.DeleteByIdAsync(aquarium.ID);

    var deletedAquarium = await unit.Aquarium.FindByIdAsync(aquarium.ID);

    Assert.IsNull(deletedAquarium);
  }

  [OneTimeTearDown]
  public async Task Teardown()
  {
    var namesToDelete = new List<string>() {
      "Create Aquarium",
      "Update Aquarium",
    };

    await unit.Aquarium.DeleteManyAsync(aqauarium => namesToDelete.Contains(aqauarium.Name));
  }
}
