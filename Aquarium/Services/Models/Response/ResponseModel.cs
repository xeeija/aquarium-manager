namespace Services.Models.Response;

public class ResponseModel
{
  // public bool HasError { get; set; }
  public bool HasError => ErrorMessages.Count > 0;

  public List<string> ErrorMessages { get; set; } = new();
}
