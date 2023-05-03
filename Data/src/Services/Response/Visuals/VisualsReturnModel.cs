using System.Runtime.Serialization;
using Newtonsoft.Json;
using NJsonSchema.Converters;

namespace DataCollector.ReturnModels.Visuals
{
  [KnownType(typeof(VisualsNumericReturnModel))]
  [KnownType(typeof(VisualsBinaryReturnModel))]
  [JsonConverter(typeof(JsonInheritanceConverter), "discriminator")]
  public abstract class VisualsReturnModel
  {
    public string Icon { get; set; }
  }
}
