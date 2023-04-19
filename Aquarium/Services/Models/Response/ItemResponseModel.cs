namespace Services.Models.Response;

public class ItemResponseModel<TItem> : ResponseModel where TItem : class
{
  public TItem Data { get; set; }
}
