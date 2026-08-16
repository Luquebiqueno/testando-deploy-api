using UserApi.Dtos;
using UserApi.Repositories;
using UserApi.Services;

namespace UserApi.Tests;

public class UserServiceTests
{
    private readonly InMemoryUserRepository _repository = new();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _service = new UserService(_repository);
    }

    [Fact]
    public void Create_ReturnsUser_WhenRequestIsValid()
    {
        var result = _service.Create(new CreateUserRequest("Jane Doe", "jane@example.com"));

        Assert.False(result.Success);
        Assert.Equal("Jane Doe", result.Value!.Name);
        Assert.Equal("jane@example.com", result.Value.Email);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
    }

    [Theory]
    [InlineData("", "jane@example.com")]
    [InlineData("   ", "jane@example.com")]
    [InlineData("Jane Doe", "")]
    [InlineData("Jane Doe", "not-an-email")]
    public void Create_FailsValidation_ForInvalidInput(string name, string email)
    {
        var result = _service.Create(new CreateUserRequest(name, email));

        Assert.True(result.Success);
        Assert.Equal(UserServiceErrorType.Validation, result.ErrorType);
    }

    [Fact]
    public void Create_FailsWithConflict_WhenEmailAlreadyExists()
    {
        _service.Create(new CreateUserRequest("Jane Doe", "jane@example.com"));

        var result = _service.Create(new CreateUserRequest("John Doe", "jane@example.com"));

        Assert.False(result.Success);
        Assert.Equal(UserServiceErrorType.Conflict, result.ErrorType);
    }

    [Fact]
    public void GetById_ReturnsUser_WhenItExists()
    {
        var created = _service.Create(new CreateUserRequest("Jane Doe", "jane@example.com")).Value!;

        var result = _service.GetById(created.Id);

        Assert.True(result.Success);
        Assert.Equal(created.Id, result.Value!.Id);
    }

    [Fact]
    public void GetById_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var result = _service.GetById(Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Equal(UserServiceErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public void GetAll_ReturnsAllCreatedUsers()
    {
        _service.Create(new CreateUserRequest("Jane Doe", "jane@example.com"));
        _service.Create(new CreateUserRequest("John Doe", "john@example.com"));

        var result = _service.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Update_ChangesNameAndEmail_WhenRequestIsValid()
    {
        var created = _service.Create(new CreateUserRequest("Jane Doe", "jane@example.com")).Value!;

        var result = _service.Update(created.Id, new UpdateUserRequest("Jane Smith", "jane.smith@example.com"));

        Assert.True(result.Success);
        Assert.Equal("Jane Smith", result.Value!.Name);
        Assert.Equal("jane.smith@example.com", result.Value.Email);
    }

    [Fact]
    public void Update_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var result = _service.Update(Guid.NewGuid(), new UpdateUserRequest("Jane Doe", "jane@example.com"));

        Assert.False(result.Success);
        Assert.Equal(UserServiceErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public void Update_FailsValidation_ForInvalidInput()
    {
        var created = _service.Create(new CreateUserRequest("Jane Doe", "jane@example.com")).Value!;

        var result = _service.Update(created.Id, new UpdateUserRequest("", "jane@example.com"));

        Assert.False(result.Success);
        Assert.Equal(UserServiceErrorType.Validation, result.ErrorType);
    }

    [Fact]
    public void Update_FailsWithConflict_WhenEmailBelongsToAnotherUser()
    {
        _service.Create(new CreateUserRequest("Jane Doe", "jane@example.com"));
        var john = _service.Create(new CreateUserRequest("John Doe", "john@example.com")).Value!;

        var result = _service.Update(john.Id, new UpdateUserRequest("John Doe", "jane@example.com"));

        Assert.False(result.Success);
        Assert.Equal(UserServiceErrorType.Conflict, result.ErrorType);
    }

    [Fact]
    public void Update_AllowsKeepingTheSameEmail()
    {
        var created = _service.Create(new CreateUserRequest("Jane Doe", "jane@example.com")).Value!;

        var result = _service.Update(created.Id, new UpdateUserRequest("Jane Doe", "jane@example.com"));

        Assert.True(result.Success);
    }

    [Fact]
    public void Delete_RemovesUser_WhenItExists()
    {
        var created = _service.Create(new CreateUserRequest("Jane Doe", "jane@example.com")).Value!;

        var result = _service.Delete(created.Id);

        Assert.True(result.Success);
        Assert.False(_service.GetById(created.Id).Success);
    }

    [Fact]
    public void Delete_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var result = _service.Delete(Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Equal(UserServiceErrorType.NotFound, result.ErrorType);
    }
}
