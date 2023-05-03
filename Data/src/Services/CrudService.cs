using DAL.MongoDB.Entities;
using DAL.MongoDB.Repository;
using DAL.MongoDB.UnitOfWork;
using Services.Response.Basis;

namespace Services;

public abstract class CrudDataService<TEntity> : DataService<TEntity> where TEntity : Entity
{
  public CrudDataService(UnitOfWork unit, IRepository<TEntity> repository, GlobalService service) : base(unit, repository, service) { }

  protected override async Task<ItemResponseModel<TEntity>> Create(TEntity entity)
  {
    var response = new ItemResponseModel<TEntity>()
    {
      Data = await Repository.InsertOneAsync(entity),
    };
    return response;
  }

  public override async Task<ItemResponseModel<TEntity>> Update(string id, TEntity entity)
  {
    var response = new ItemResponseModel<TEntity>()
    {
      Data = await Repository.UpdateOneAsync(entity),
    };
    return response;
  }
}
