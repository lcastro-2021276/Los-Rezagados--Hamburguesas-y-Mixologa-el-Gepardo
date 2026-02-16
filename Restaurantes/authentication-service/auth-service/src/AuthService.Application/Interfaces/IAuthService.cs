using AuthService.Application.DTOs;

namespace AuthService.Application.Interfaces;

public interface IAuthService
{
    AuthResponseDto Login(LoginDto dto);
    AuthResponseDto Register(RegisterDto dto);
}
