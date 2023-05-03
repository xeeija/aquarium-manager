namespace DataCollector.ReturnModels.Visuals
{
  public class VisualsNumericReturnModel : VisualsReturnModel
  {
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public string Unit { get; set; }
  }
}
