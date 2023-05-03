using Context.DAL.Visuals;
using DAL.MongoDB.UnitOfWork;

namespace DAL.MongoDB.Repository
{
  public class VisualsRepository : Repository<DataPointVisual>, IVisualsRepository
  {
    public VisualsRepository(DBContext context) : base(context) { }

    public DataPointVisual GetDataPointsVisuals(string name)
    {
      return FindOne(x => x.Name.Equals(name));
    }
  }
}
