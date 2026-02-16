using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IJwtService _jwt;

    public AuthService(IUserRepository users, IJwtService jwt)
    {
        _users = users;
        _jwt = jwt;
    }

    public AuthResponseDto Login(LoginDto dto)
    {
        var user = _users.GetByUsername(dto.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return new AuthResponseDto { Success = false, Message = "Credenciales inválidas" };
        }

        return new AuthResponseDto
        {
            Success = true,
            Message = "Login exitoso",
            Token = _jwt.GenerateToken(user),
            User = new UserDetailsDto { Id = user.Id, Username = user.Username, Role = user.Role }
        };
    }

    public AuthResponseDto Register(RegisterDto dto)
    {
        // Validaciones mínimas
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return new AuthResponseDto { Success = false, Message = "Username y Password son requeridos" };
        }

        // Evitar duplicados
        var exists = _users.GetByUsername(dto.Username);
        if (exists != null)
        {
            return new AuthResponseDto { Success = false, Message = "El usuario ya existe" };
        }

        var newUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = dto.Username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = string.IsNullOrWhiteSpace(dto.Role) ? "USER_ROLE" : dto.Role.Trim()
        };

        _users.Add(newUser);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Registro exitoso",
            Token = _jwt.GenerateToken(newUser),
            User = new UserDetailsDto { Id = newUser.Id, Username = newUser.Username, Role = newUser.Role }
        };
    }
}
