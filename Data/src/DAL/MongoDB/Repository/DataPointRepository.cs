using System.Linq.Expressions;
using DAL.MongoDB.Entities;
using DAL.MongoDB.UnitOfWork;

namespace DAL.MongoDB.Repository;

public class DataPointRepository : IDataPointRepository
{
  private DBContext context;

  public DataPointRepository(DBContext context)
  {
    this.context = context;
  }
  public Task DeleteByIdAsync(string id)
  {
    throw new NotImplementedException();
  }

  public Task DeleteManyAsync(Expression<Func<DataPoint, bool>> filterExpression)
  {
    throw new NotImplementedException();
  }

  public Task DeleteOneAsync(Expression<Func<DataPoint, bool>> filterExpression)
  {
    throw new NotImplementedException();
  }

  public List<DataPoint> FilterBy(Expression<Func<DataPoint, bool>> filterExpression)
  {
    throw new NotImplementedException();
  }

  public List<TProjected> FilterBy<TProjected>(Expression<Func<DataPoint, bool>> filterExpression, Expression<Func<DataPoint, TProjected>> projectionExpression)
  {
    throw new NotImplementedException();
  }

  public Task<List<DataPoint>> FilterByAsync(Expression<Func<DataPoint, bool>> filterExpression)
  {
    throw new NotImplementedException();
  }

  public Task<DataPoint> FindByIdAsync(string id)
  {
    throw new NotImplementedException();
  }

  public Task<DataPoint> FindOneAsync(Expression<Func<DataPoint, bool>> filterExpression)
  {
    throw new NotImplementedException();
  }

  public List<DataPoint> GetDataPointsForDevice(DeviceType deviceType)
  {
    throw new NotImplementedException();
  }

  public Task InsertManyAsync(List<DataPoint> document)
  {
    throw new NotImplementedException();
  }

  public Task<DataPoint> InsertOneAsync(DataPoint document)
  {
    throw new NotImplementedException();
  }

  public Task<DataPoint> InsertOrUpdateOneAsync(DataPoint document)
  {
    throw new NotImplementedException();
  }

  public Task<DataPoint> UpdateOneAsync(DataPoint document)
  {
    throw new NotImplementedException();
  }
}
