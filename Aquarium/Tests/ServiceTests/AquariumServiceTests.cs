using DAL.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Services;

namespace Tests.ServiceTests;

public class AquariumServiceTests : BaseUnitOfWorkTests
{
  private AquariumService aquariumService;

  public AquariumServiceTests() : base()
  {
    aquariumService = new AquariumService(unit, unit.Aquarium, null);
  }

  [Test]
  public async Task ShouldGetAquariumsForUser()
  {
    var modelState = new Mock<ModelStateDictionary>();
    await aquariumService.SetModelState(modelState.Object);

    var user = await unit.User.Register(new User()
    {
      FirstName = "Aquaman",
      LastName = "Aquaman",
      Username = "aquaman123",
      Email = "aqua@man.com",
      Password = "Secur3P.ssWD",
      IsActive = true,
    });

    var aquarium = await unit.Aquarium.InsertOneAsync(new Aquarium()
    {
      Liters = 500,
      Height = 55,
      Depth = 65,
      Length = 150,
      WaterType = WaterType.SaltWater,
      Name = "Aquarium Test 1",
    });
    var aquarium2 = await unit.Aquarium.InsertOneAsync(new Aquarium()
    {
      Liters = 500,
      Height = 55,
      Depth = 65,
      Length = 150,
      WaterType = WaterType.SaltWater,
      Name = "Aquarium Test 2",
    });
    await unit.Aquarium.InsertOneAsync(new Aquarium()
    {
      Liters = 500,
      Height = 55,
      Depth = 65,
      Length = 150,
      WaterType = WaterType.SaltWater,
      Name = "Aquarium Test 3",
    });

    await unit.UserAquarium.InsertOneAsync(new UserAquarium()
    {
      UserID = user.ID,
      AquariumID = aquarium.ID,
      Role = UserRole.Admin,
    });
    await unit.UserAquarium.InsertOneAsync(new UserAquarium()
    {
      UserID = user.ID,
      AquariumID = aquarium2.ID,
      Role = UserRole.Admin,
    });

    var response = await aquariumService.GetForUser(user.ID);

    Assert.NotNull(response);
    Assert.False(response.HasError);
    Assert.AreEqual(response.Data?.Count, 2);
  }

  [OneTimeTearDown]
  public async Task Teardown()
  {
    var user = await unit.User.FindOneAsync(u => u.Username == "aquaman123");

    await unit.Aquarium.DeleteManyAsync(aqauarium => aqauarium.Name.StartsWith("Aquarium Test"));
    await unit.UserAquarium.DeleteManyAsync(item => item.UserID == user.ID);
    await unit.User.DeleteByIdAsync(user.ID);
  }
}
