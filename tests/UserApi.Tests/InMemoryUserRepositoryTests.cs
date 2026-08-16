using UserApi.Models;
using UserApi.Repositories;

namespace UserApi.Tests;

public class InMemoryUserRepositoryTests
{
    private readonly InMemoryUserRepository _repository = new();

    private static User NewUser(string email = "jane@example.com") => new()
    {
        Id = Guid.NewGuid(),
        Name = "Jane Doe",
        Email = email,
        CreatedAt = DateTime.UtcNow
    };

    [Fact]
    public void Add_StoresUser_SoItCanBeRetrievedById()
    {
        var user = NewUser();

        _repository.Add(user);

        Assert.Equal(user, _repository.GetById(user.Id));
    }

    [Fact]
    public void GetById_ReturnsNull_WhenUserDoesNotExist()
    {
        Assert.Null(_repository.GetById(Guid.NewGuid()));
    }

    [Fact]
    public void GetAll_ReturnsEveryStoredUser()
    {
        _repository.Add(NewUser("a@example.com"));
        _repository.Add(NewUser("b@example.com"));

        Assert.Equal(2, _repository.GetAll().Count);
    }

    [Fact]
    public void Update_ReturnsFalse_WhenUserDoesNotExist()
    {
        Assert.False(_repository.Update(NewUser()));
    }

    [Fact]
    public void Update_ReplacesExistingUser_AndReturnsTrue()
    {
        var user = NewUser();
        _repository.Add(user);

        user.Name = "Updated Name";
        var updated = _repository.Update(user);

        Assert.True(updated);
        Assert.Equal("Updated Name", _repository.GetById(user.Id)!.Name);
    }

    [Fact]
    public void Delete_RemovesUser_AndReturnsTrue()
    {
        var user = NewUser();
        _repository.Add(user);

        Assert.True(_repository.Delete(user.Id));
        Assert.Null(_repository.GetById(user.Id));
    }

    [Fact]
    public void Delete_ReturnsFalse_WhenUserDoesNotExist()
    {
        Assert.False(_repository.Delete(Guid.NewGuid()));
    }

    [Fact]
    public void ExistsByEmail_IsCaseInsensitive()
    {
        _repository.Add(NewUser("jane@example.com"));

        Assert.True(_repository.ExistsByEmail("JANE@EXAMPLE.COM"));
    }

    [Fact]
    public void ExistsByEmail_ExcludesGivenId()
    {
        var user = NewUser("jane@example.com");
        _repository.Add(user);

        Assert.False(_repository.ExistsByEmail("jane@example.com", excludeId: user.Id));
    }
}
