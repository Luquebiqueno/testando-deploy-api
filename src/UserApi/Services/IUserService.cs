using UserApi.Dtos;

namespace UserApi.Services;

public interface IUserService
{
    IReadOnlyList<UserResponse> GetAll();
    UserServiceResult<UserResponse> GetById(Guid id);
    UserServiceResult<UserResponse> Create(CreateUserRequest request);
    UserServiceResult<UserResponse> Update(Guid id, UpdateUserRequest request);
    UserServiceResult<bool> Delete(Guid id);
}
