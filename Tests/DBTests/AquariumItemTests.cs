using DAL.Entities;

namespace Tests.DBTests;

public class AquariumItemTests : BaseUnitOfWorkTests
{

  [Test]
  public void ShouldGetAnimals()
  {
    Task.WaitAll(
      unit.AquariumItem.InsertOneAsync(new Animal()
      {
        Name = "Nemo",
        IsAlive = true,
        Amount = 1,
        Species = "Clownfish",
        Description = "test item",
      }),
      unit.AquariumItem.InsertOneAsync(new Animal()
      {
        Name = "Dorie",
        IsAlive = true,
        Amount = 2,
        Species = "Surgeonfish",
        Description = "test item",
      }),
      unit.AquariumItem.InsertOneAsync(new Coral()
      {
        Name = "Corina",
        Amount = 3,
        CoralType = CoralType.SoftCoral,
        Species = "Anthelia",
        Description = "test item",
      })
    );

    var animals = unit.AquariumItem.GetAnimals();
    animals.Sort((a, b) => a.Name.CompareTo(b.Name));

    Assert.AreEqual(animals.Count, 2);
    Assert.AreEqual(animals.Select(a => a.Name), new List<string>() { "Dorie", "Nemo" });

  }

  [OneTimeTearDown]
  public async Task Teardown()
  {
    await unit.AquariumItem.DeleteManyAsync(item => item.Description == "test item");
  }
}
