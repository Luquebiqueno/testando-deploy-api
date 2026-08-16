namespace UserApi.Dtos;

public record CreateUserRequest(string Name, string Email);

public record UpdateUserRequest(string Name, string Email);

public record UserResponse(Guid Id, string Name, string Email, DateTime CreatedAt);
