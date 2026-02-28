using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Persistence.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new()
    {
        new User
        {
            Id = Guid.NewGuid().ToString(), // 
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Rezagados123!*"),
            Role = "ADMIN",
            EmailConfirmed = true
        }
    };

    // ======= MÉTODOS SINCRÓNICOS =======

    public User? GetByUsername(string username)
        => _users.FirstOrDefault(u => u.Username == username);

    public User? GetByEmail(string email)
        => _users.FirstOrDefault(u => u.Email == email);

    public User? GetByVerificationToken(string token)
        => _users.FirstOrDefault(u => u.EmailVerificationToken == token);

    public User? GetByResetToken(string token)
        => _users.FirstOrDefault(u => u.PasswordResetToken == token);

    public User Add(User user)
    {
        if (string.IsNullOrEmpty(user.Id))
            user.Id = Guid.NewGuid().ToString(); // 

        _users.Add(user);
        return user;
    }

    public void Update(User user)
    {
        var index = _users.FindIndex(u => u.Id == user.Id);
        if (index != -1)
            _users[index] = user;
    }

    // ======= MÉTODOS ASÍNCRONOS =======

    public Task<User?> GetByIdAsync(Guid id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id.ToString());
        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        var user = _users.FirstOrDefault(u => u.Email == email);
        return Task.FromResult(user);
    }

    public Task AddAsync(User user)
    {
        Add(user);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user)
    {
        Update(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user)
    {
        _users.RemoveAll(u => u.Id == user.Id);
        return Task.CompletedTask;
    }
}