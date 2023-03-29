using DAL.Entities;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using Services;
using Services.Models.Request;

namespace Tests.ServiceTests;

public class PictureServiceTests : BaseUnitOfWorkTests
{
  private PictureService pictureService;

  public PictureServiceTests() : base()
  {
    pictureService = new PictureService(unit, unit.Picture, null);
  }

  [Test]
  [Order(0)]
  public async Task ShouldUploadPicture()
  {
    var aquarium = await unit.Aquarium.InsertOneAsync(new Aquarium()
    {
      Liters = 500,
      Height = 55,
      Depth = 65,
      Length = 150,
      WaterType = WaterType.SaltWater,
      Name = "picture service test 1",
    });

    Assert.NotNull(aquarium);
    Assert.NotNull(aquarium?.ID);

    var pictureBytes = await File.ReadAllBytesAsync(@"..\..\..\img\fish.jpg");
    var file = new FormFile(new MemoryStream(pictureBytes), 0, pictureBytes.Length, "Fish", "fish.jpg");

    var pictureRequest = new PictureRequest()
    {
      Description = "picture service test",
      FormFile = file,
    };

    var picturesBefore = unit.Picture.FilterBy(_ => true).Count();

    var response = await pictureService.AddPicture(aquarium?.ID ?? "", pictureRequest);

    var picturesAfter = unit.Picture.FilterBy(_ => true).Count();

    Assert.AreEqual(picturesAfter, picturesBefore + 1);
    Assert.NotNull(response);
    Assert.IsFalse(response.HasError);

  }

  [OneTimeTearDown]
  public async Task Teardown()
  {
    await unit.Aquarium.DeleteManyAsync(aqauarium => aqauarium.Name.StartsWith("picture service test"));

    var createPictures = unit.Picture.FilterBy(pic => pic.Description == "picture service test");

    log.Debug(createPictures.ToJson());

    // picture => unit.Context.GridFSBucket.DeleteAsync(picture.PictureID)
    var deleteTasks = createPictures.Select(async picture => await pictureService.Delete(picture.PictureID)).ToArray();
    Task.WaitAll(deleteTasks);

    unit.Picture.DeleteManyAsync(pic => pic.Description == "picture service test");
  }

}
