using DAL.Entities;

namespace Services.Models.Response;

public class PictureResponse
{
  public Picture Picture { get; set; }

  public byte[] Bytes { get; set; }

  public string Base64 { get => Bytes != null ? Convert.ToBase64String(Bytes) : ""; }
}
