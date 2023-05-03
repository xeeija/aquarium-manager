using Context.DAL.Visuals;

namespace DAL.MongoDB.Repository
{
  public interface IVisualsRepository : IRepository<DataPointVisual>
  {
    DataPointVisual GetDataPointsVisuals(string name);
  }
}
