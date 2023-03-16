using System.Linq.Expressions;
using DAL.Entities;
using MongoDB.Driver;
using Serilog;
using Utils;

namespace DAL.Repository;

public class Repository<E> : IRepository<E> where E : Entity
{

  protected ILogger log = Logger.ContextLog<Repository<E>>();

  private readonly IMongoCollection<E> collection;

  public Repository(DBContext context)
  {
    collection = context.DataBase.GetCollection<E>(typeof(E).Name);
  }

  public async Task DeleteByIdAsync(string id)
  {
    await DeleteOneAsync((document) => document.ID == id);
    // await collection.FindOneAndDeleteAsync((document) => document.ID == id);
  }

  public async Task DeleteManyAsync(Expression<Func<E, bool>> filterFn)
  {
    await collection.DeleteManyAsync(filterFn);
  }

  public async Task DeleteOneAsync(Expression<Func<E, bool>> filterFn)
  {
    await collection.FindOneAndDeleteAsync(filterFn);
  }

  public IEnumerable<E> FilterBy(Expression<Func<E, bool>> filterFn)
  {
    return collection.Find(filterFn).ToEnumerable();
  }

  public IEnumerable<TProjected> FilterBy<TProjected>(Expression<Func<E, bool>> filterFn, Expression<Func<E, TProjected>> projectFn)
  {
    return collection.Find(filterFn).Project(projectFn).ToEnumerable();
  }

  public Task<E> FindByIdAsync(string id)
  {
    // TODO: Is await here necessary, or is it enough to await the task in FindOneAsync?
    return FindOneAsync((document) => document.ID == id);
  }

  public async Task<E> FindOneAsync(Expression<Func<E, bool>> filterFn)
  {
    var entitites = await collection.FindAsync(filterFn);

    return entitites.FirstOrDefault();
  }

  public async Task<E> InsertOneAsync(E document)
  {
    document.ID = document.GenerateID();
    await collection.InsertOneAsync(document);

    return document;
  }

  public async Task<E> InsertOrUpdateOneAsync(E document)
  {
    var entity = await FindOneAsync(doc => doc.ID == document.ID);

    return await (entity == null ? InsertOneAsync(document) : UpdateOneAsync(document));
  }

  public async Task<E> UpdateOneAsync(E document)
  {
    await collection.FindOneAndReplaceAsync((doc) => doc.ID == document.ID, document);
    return document;

    // var updatedEntity = await collection.FindOneAndReplaceAsync((doc) => doc.ID == document.ID, document);

    // log.Information("document");
    // log.Information(document.ToJson());

    // // TODO: Why is updatedEntity not updated, even though its update in the DB?
    // log.Information("updated");
    // log.Information(updatedEntity.ToJson());
    // return updatedEntity;
  }
}
