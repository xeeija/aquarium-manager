using DAL;
using DAL.Entities;
using DAL.Repository;
using MimeKit;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using Services.Models.Request;
using Services.Models.Response;

namespace Services;

public class PictureService : CrudService<Picture>
{
  public PictureService(UnitOfWork unit, IRepository<Picture> repository, GlobalService service) : base(unit, repository, service) { }

  public async Task<ItemResponseModel<PictureResponse>> AddPicture(string aquariumId, PictureRequest request)
  {
    var response = new ItemResponseModel<PictureResponse>();

    if (String.IsNullOrEmpty(aquariumId))
    {
      response.ErrorMessages.Add("No aquarium provided");
      return response;
    }

    if (request.FormFile == null)
    {
      response.ErrorMessages.Add("No picture provided");
      return response;
    }

    var filename = request.FormFile.FileName;

    if (String.IsNullOrEmpty(filename))
    {
      response.ErrorMessages.Add("Filename is empty");
      return response;
    }

    var type = MimeTypes.GetMimeType(filename);

    if (!type.StartsWith("image/"))
    {
      response.ErrorMessages.Add("Only images are allowed");
      return response;
    }

    byte[] pictureBytes = null;

    using (var stream = new MemoryStream())
    {
      request.FormFile.CopyTo(stream);
      pictureBytes = stream.ToArray();
    }

    var pictureId = await unit.Context.GridFSBucket.UploadFromBytesAsync(filename, pictureBytes);

    var picture = new Picture()
    {
      PictureID = pictureId.ToString(),
      Description = request.Description,
      AquariumID = aquariumId,
      ContentType = type,
    };

    var pictureSaved = await unit.Picture.InsertOneAsync(picture);

    var pictureBytesSaved = await unit.Context.GridFSBucket.DownloadAsBytesAsync(pictureId);

    response.Data = new PictureResponse()
    {
      Picture = pictureSaved,
      Bytes = pictureBytesSaved,
    };

    return response;

  }

  public async Task<ItemResponseModel<PictureResponse>> GetPicture(string id)
  {
    var response = new ItemResponseModel<PictureResponse>();

    var picture = await repository.FindByIdAsync(id);


    if (picture == null)
    {
      response.ErrorMessages.Add("Picture is null");
      return response;
    };

    var pictureBytes = await unit.Context.GridFSBucket.DownloadAsBytesAsync(picture.PictureID);

    response.Data = new PictureResponse()
    {
      Picture = picture,
      Bytes = pictureBytes,
    };

    return response;
  }

  public async Task<ItemResponseModel<List<PictureResponse>>> GetForAquarium(string aquariumId)
  {
    var response = new ItemResponseModel<List<PictureResponse>>();

    var pictures = repository.FilterBy(picture => picture.AquariumID == aquariumId).ToList();

    pictures.ForEach(async picture =>
    {
      var pictureResponse = await GetPicture(picture.ID);

      if (pictureResponse.ErrorMessages.Count > 0)
      {
        response.ErrorMessages.AddRange(pictureResponse.ErrorMessages);
      }

      response.Data.Add(pictureResponse.Data);
    });

    return response;
  }

  public override async Task<ActionResponseModel> Delete(string pictureId)
  {
    var picture = await repository.FindByIdAsync(pictureId);
    try
    {
      await unit.Context.GridFSBucket.DeleteAsync(picture.PictureID);
    }
    catch (Exception ex)
    {
      if (ex is GridFSFileNotFoundException)
      {
        log.Warning($"PictureID {picture.PictureID} not found in GridFS");
      }
      else
      {
        return new ActionResponseModel()
        {
          Success = false,
          ErrorMessages = new List<string>() { ex.Message },
        };
      }
    }

    return await base.Delete(pictureId);
  }

  public override async Task<bool> Validate(Picture entity)
  {
    if (entity == null)
    {
      modelStateWrapper.AddError("Picture empty", "Picture is empty");
    }

    return modelStateWrapper.IsValid;
  }
}
