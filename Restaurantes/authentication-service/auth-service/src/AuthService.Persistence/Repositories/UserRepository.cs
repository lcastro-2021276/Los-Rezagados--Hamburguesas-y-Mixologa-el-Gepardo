using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;

namespace AuthService.Persistence.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new()
    {
        new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Rezagados123!*"),
            Role = "ADMIN",
            EmailConfirmed = true
        }
    };

    // ========================= GETTERS =========================

    public Task<User?> GetByUsername(string username)
    {
        var user = _users.FirstOrDefault(u => u.Username == username);
        return Task.FromResult(user);
    }

    public Task<User?> GetByEmail(string email)
    {
        var user = _users.FirstOrDefault(u => u.Email == email);
        return Task.FromResult(user);
    }

    public Task<User?> GetByVerificationToken(string token)
    {
        var user = _users.FirstOrDefault(u => u.EmailVerificationToken == token);
        return Task.FromResult(user);
    }

    public Task<User?> GetByResetToken(string token)
    {
        var user = _users.FirstOrDefault(u => u.PasswordResetToken == token);
        return Task.FromResult(user);
    }

    // ========================= COMMANDS =========================

    public Task Add(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
            user.Id = Guid.NewGuid().ToString();

        _users.Add(user);
        return Task.CompletedTask;
    }

    public Task Update(User user)
    {
        var index = _users.FindIndex(u => u.Id == user.Id);
        if (index != -1)
            _users[index] = user;

        return Task.CompletedTask;
    }
}