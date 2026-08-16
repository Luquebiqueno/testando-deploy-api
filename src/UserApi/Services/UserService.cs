using System.Text.RegularExpressions;
using UserApi.Dtos;
using UserApi.Models;
using UserApi.Repositories;

namespace UserApi.Services;

public partial class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<UserResponse> GetAll() =>
        _repository.GetAll().Select(ToResponse).ToList();

    public UserServiceResult<UserResponse> GetById(Guid id)
    {
        var user = _repository.GetById(id);
        return user is null
            ? UserServiceResult<UserResponse>.Fail(UserServiceErrorType.NotFound, $"User '{id}' was not found.")
            : UserServiceResult<UserResponse>.Ok(ToResponse(user));
    }

    public UserServiceResult<UserResponse> Create(CreateUserRequest request)
    {
        var validationError = Validate(request.Name, request.Email);
        if (validationError is not null)
        {
            return UserServiceResult<UserResponse>.Fail(UserServiceErrorType.Validation, validationError);
        }

        if (_repository.ExistsByEmail(request.Email))
        {
            return UserServiceResult<UserResponse>.Fail(
                UserServiceErrorType.Conflict,
                $"A user with email '{request.Email}' already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _repository.Add(user);
        return UserServiceResult<UserResponse>.Ok(ToResponse(user));
    }

    public UserServiceResult<UserResponse> Update(Guid id, UpdateUserRequest request)
    {
        var existing = _repository.GetById(id);
        if (existing is null)
        {
            return UserServiceResult<UserResponse>.Fail(UserServiceErrorType.NotFound, $"User '{id}' was not found.");
        }

        var validationError = Validate(request.Name, request.Email);
        if (validationError is not null)
        {
            return UserServiceResult<UserResponse>.Fail(UserServiceErrorType.Validation, validationError);
        }

        if (_repository.ExistsByEmail(request.Email, excludeId: id))
        {
            return UserServiceResult<UserResponse>.Fail(
                UserServiceErrorType.Conflict,
                $"A user with email '{request.Email}' already exists.");
        }

        existing.Name = request.Name.Trim();
        existing.Email = request.Email.Trim();
        _repository.Update(existing);

        return UserServiceResult<UserResponse>.Ok(ToResponse(existing));
    }

    public UserServiceResult<bool> Delete(Guid id)
    {
        var deleted = _repository.Delete(id);
        return deleted
            ? UserServiceResult<bool>.Ok(true)
            : UserServiceResult<bool>.Fail(UserServiceErrorType.NotFound, $"User '{id}' was not found.");
    }

    private static string? Validate(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Name is required.";
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return "Email is required.";
        }

        if (!EmailRegex().IsMatch(email))
        {
            return "Email is not a valid email address.";
        }

        return null;
    }

    private static UserResponse ToResponse(User user) =>
        new(user.Id, user.Name, user.Email, user.CreatedAt);

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
