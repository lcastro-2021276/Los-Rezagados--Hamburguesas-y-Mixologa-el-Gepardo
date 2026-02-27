using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using System;

namespace AuthService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IJwtService _jwt;
        private readonly IEmailService? _email;

        public AuthService(
            IUserRepository users,
            IJwtService jwt,
            IEmailService? email = null
        )
        {
            _users = users;
            _jwt = jwt;
            _email = email;
        }

        // ========================= LOGIN =========================
        public AuthResponseDto Login(LoginDto dto)
        {
            var user = _users.GetByUsername(dto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Credenciales inválidas"
                };
            }

            if (!user.EmailConfirmed)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email no verificado"
                };
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login exitoso",
                Token = _jwt.GenerateToken(user),
                User = new UserDetailsDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role
                }
            };
        }

        // ========================= REGISTER =========================
        public AuthResponseDto Register(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Password) ||
                string.IsNullOrWhiteSpace(dto.Email))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Username, Email y Password son requeridos"
                };
            }

            if (_users.GetByUsername(dto.Username) != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "El usuario ya existe"
                };
            }

            if (_users.GetByEmail(dto.Email) != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "El email ya está registrado"
                };
            }

            var verificationToken = Guid.NewGuid().ToString();

            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = dto.Username.Trim(),
                Email = dto.Email.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = string.IsNullOrWhiteSpace(dto.Role) ? "USER" : dto.Role.Trim(),
                EmailConfirmed = false,
                EmailVerificationToken = verificationToken
            };

            _users.Add(newUser);

            return new AuthResponseDto
            {
                Success = true,
                Message = $"Registro exitoso. Verifica tu correo con el token: {verificationToken}",
                Token = null, // ✅ NO JWT hasta verificar email
                User = new UserDetailsDto
                {
                    Id = newUser.Id,
                    Username = newUser.Username,
                    Role = newUser.Role
                }
            };
        }

        // ========================= VERIFY EMAIL =========================
        public AuthResponseDto VerifyEmail(string token)
        {
            var user = _users.GetByVerificationToken(token);

            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Token inválido"
                };
            }

            user.EmailConfirmed = true;
            user.EmailVerificationToken = null;
            _users.Update(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Email verificado correctamente"
            };
        }

        // ========================= FORGOT PASSWORD =========================
        public AuthResponseDto ForgotPassword(string email)
        {
            var user = _users.GetByEmail(email);

            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            user.PasswordResetToken = Guid.NewGuid().ToString();
            _users.Update(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = $"Token de recuperación generado: {user.PasswordResetToken}"
            };
        }

        // ========================= RESET PASSWORD =========================
        public AuthResponseDto ResetPassword(string token, string newPassword)
        {
            var user = _users.GetByResetToken(token);

            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Token inválido"
                };
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordResetToken = null;
            _users.Update(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Contraseña reseteada correctamente"
            };
        }
    }
}