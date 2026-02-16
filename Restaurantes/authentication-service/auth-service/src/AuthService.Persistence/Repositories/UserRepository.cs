using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using BCrypt.Net;

namespace AuthService.Persistence.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users =
    [
        new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Rezagados123!*"),
            Role = "adminRestaurante"
        }
    ];

    public User? GetByUsername(string username)
        => _users.FirstOrDefault(u => u.Username == username);

    public User Add(User user)
    {
        _users.Add(user);
        return user;
    }
}
