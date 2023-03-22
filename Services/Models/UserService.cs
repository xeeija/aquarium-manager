using DAL;
using DAL.Entities;
using DAL.Repository;
using Services.Models.Request;
using Services.Models.Response;

namespace Services.Models;

public class UserService : Service<User>
{
  public UserService(UnitOfWork unit, IRepository<User> repository, GlobalService service) : base(unit, repository, service) { }

  public override async Task<ItemResponseModel<User>> Create(User entity)
  {
    var response = new ItemResponseModel<User>()
    {
      Data = await repository.InsertOneAsync(entity),
      // HasError = false,
    };
    return response;
  }

  public override async Task<ItemResponseModel<User>> Update(string id, User entity)
  {
    var response = new ItemResponseModel<User>()
    {
      Data = await repository.UpdateOneAsync(entity),
      // HasError = false,
    };
    return response;
  }

  public async Task<ItemResponseModel<UserResponse>> Login(LoginRequest request)
  {
    throw new NotImplementedException();
  }

  public override async Task<bool> Validate(User entity)
  {
    if (entity == null)
    {
      modelStateWrapper.AddError("No user", "No user provided");
      return modelStateWrapper.IsValid;
    }

    if (String.IsNullOrEmpty(entity.Email))
    {
      modelStateWrapper.AddError("No email", "Please provide an email");
    }

    var userEmailExists = await this.repository.FindOneAsync(user => user.Email == entity.Email && entity.ID != user.ID);
    if (userEmailExists != null)
    {
      modelStateWrapper.AddError("Email exists", "This email is already taken");
    }

    if (String.IsNullOrEmpty(entity.FirstName))
    {
      modelStateWrapper.AddError("No email", "Please provide a firstname");
    }
    if (String.IsNullOrEmpty(entity.LastName))
    {
      modelStateWrapper.AddError("No email", "Please provide a lastname");
    }

    return modelStateWrapper.IsValid;
  }
}
