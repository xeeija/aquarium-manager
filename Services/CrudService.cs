using DAL;
using DAL.Entities;
using DAL.Repository;
using Services.Models.Response;

namespace Services;

public abstract class CrudService<TEntity> : Service<TEntity> where TEntity : Entity
{
  public CrudService(UnitOfWork unit, IRepository<TEntity> repository, GlobalService service) : base(unit, repository, service) { }

  public override async Task<ItemResponseModel<TEntity>> Create(TEntity entity)
  {
    var response = new ItemResponseModel<TEntity>()
    {
      Data = await repository.InsertOneAsync(entity),
    };
    return response;
  }

  public override async Task<ItemResponseModel<TEntity>> Update(string id, TEntity entity)
  {
    var response = new ItemResponseModel<TEntity>()
    {
      Data = await repository.UpdateOneAsync(entity),
    };
    return response;
  }
}
