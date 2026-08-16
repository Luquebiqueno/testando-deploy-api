using UserApi.Models;

namespace UserApi.Repositories;

public interface IUserRepository
{
    IReadOnlyList<User> GetAll();
    User? GetById(Guid id);
    User Add(User user);
    bool Update(User user);
    bool Delete(Guid id);
    bool ExistsByEmail(string email, Guid? excludeId = null);
}
