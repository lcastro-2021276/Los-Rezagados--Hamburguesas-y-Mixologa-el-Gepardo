using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces;

public interface IUserRepository
{
    User? GetByUsername(string username);
    User Add(User user);
}
