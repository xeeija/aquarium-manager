using DAL;
using DAL.Entities;
using DAL.Repository;
using Services.Auth;
using Services.Models.Request;
using Services.Models.Response;

namespace Services;

public class UserService : CrudService<User>
{
  // protected new IUserRepository repository;

  // public UserService(UnitOfWork unit, IUserRepository repository, GlobalService service) : base(unit, repository, service)
  // {
  //   this.repository = repository;
  // }

  public UserService(UnitOfWork unit, IRepository<User> repository, GlobalService service) : base(unit, repository, service) { }

  public async Task<ItemResponseModel<UserResponse>> Login(LoginRequest request)
  {
    var response = new ItemResponseModel<UserResponse>();

    // var user = await repository.Login(request.Username, request.Password);
    var user = await unit.User.Login(request.Username, request.Password);

    if (user == null)
    {
      response.ErrorMessages.Add("Username or password is incorrect");
      return response;
    }

    var auth = new Authentication(unit);
    var authInfo = await auth.Authenticate(user);

    if (authInfo == null)
    {
      response.ErrorMessages.Add("Failed to authenticate");
      return response;
    }

    response.Data = new UserResponse()
    {
      AuthInfo = authInfo,
      User = user
    };

    return response;

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
    else
    {
      var userEmailExists = await repository.FindOneAsync(user => user.Email == entity.Email && entity.ID != user.ID);
      if (userEmailExists != null)
      {
        modelStateWrapper.AddError("Email exists", "This email is already taken");
      }
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
