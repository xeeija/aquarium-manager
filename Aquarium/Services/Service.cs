using DAL;
using DAL.Entities;
using DAL.Repository;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Services.Models.Response;
using Services.Utils;
using Utils;

namespace Services;

public abstract class Service<TEntity> where TEntity : Entity
{
  protected UnitOfWork unit;
  protected IRepository<TEntity> repository;
  protected GlobalService globalService;
  protected ModelStateWrapper modelStateWrapper;
  protected ModelStateDictionary validation;

  protected Serilog.ILogger log = Logger.ContextLog<Service<TEntity>>();

  protected User CurrentUser { get; private set; }

  public Service(UnitOfWork unit, IRepository<TEntity> repository, GlobalService service)
  {
    this.unit = unit;
    this.repository = repository;
    this.globalService = service;
  }

  public async Task Load(string email)
  {
    CurrentUser = await unit.User.FindOneAsync(u => u.Email.ToLower() == email.ToLower());
  }

  public virtual async Task<ActionResponseModel> Delete(string id)
  {
    await repository.DeleteByIdAsync(id);

    var response = new ActionResponseModel() { Success = true };
    return response;
  }

  public abstract Task<ItemResponseModel<TEntity>> Create(TEntity entity);

  public abstract Task<ItemResponseModel<TEntity>> Update(string id, TEntity entity);

  public abstract Task<bool> Validate(TEntity entity);

  public virtual async Task<ItemResponseModel<TEntity>> CreateHandler(TEntity entity)
  {
    var response = new ItemResponseModel<TEntity>();

    if (!await Validate(entity))
    {
      // response.HasError = true;
      response.ErrorMessages = modelStateWrapper.Errors.Values.ToList();
      return response;
    }

    var createdEntity = await Create(entity);

    if (createdEntity == null)
    {
      // response.HasError = true;
      response.ErrorMessages.Add("Created entity is empty");
      return response;
    }

    return createdEntity;
  }

  public virtual async Task<ItemResponseModel<TEntity>> UpdateHandler(string id, TEntity entity)
  {
    var response = new ItemResponseModel<TEntity>();

    if (!await Validate(entity))
    {
      // response.HasError = true;
      response.ErrorMessages = modelStateWrapper.Errors.Values.ToList();
      return response;
    }

    var updatedEntity = await Update(id, entity);

    if (updatedEntity == null)
    {
      // response.HasError = true;
      response.ErrorMessages.Add("Updated entity is empty");
      return response;
    }

    // TODO: ?
    if (!updatedEntity.HasError)
    {
      // what? why?
      updatedEntity.Data.ID = id;
      // await this.repository.UpdateOneAsync(entity);
      // updatedEntity.HasError = false;
      return updatedEntity;
    }

    return response;
  }

  public async virtual Task<TEntity> Get(string id)
  {
    return await repository.FindByIdAsync(id);
  }

  public async virtual Task<List<TEntity>> Get()
  {
    return repository.FilterBy(_ => true).ToList();
  }


  public async Task SetModelState(ModelStateDictionary validate)
  {
    modelStateWrapper = new ModelStateWrapper(validate);
    validation = validate;
  }

}
