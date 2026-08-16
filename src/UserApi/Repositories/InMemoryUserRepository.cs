using System.Collections.Concurrent;
using UserApi.Models;

namespace UserApi.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, User> _users = new();

    public IReadOnlyList<User> GetAll() => _users.Values.ToList();

    public User? GetById(Guid id) => _users.GetValueOrDefault(id);

    public User Add(User user)
    {
        _users[user.Id] = user;
        return user;
    }

    public bool Update(User user)
    {
        if (!_users.ContainsKey(user.Id))
        {
            return false;
        }

        _users[user.Id] = user;
        return true;
    }

    public bool Delete(Guid id) => _users.TryRemove(id, out _);

    public bool ExistsByEmail(string email, Guid? excludeId = null) =>
        _users.Values.Any(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
            u.Id != excludeId);
}
