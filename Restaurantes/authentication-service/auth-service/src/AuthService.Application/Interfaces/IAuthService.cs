using AuthService.Application.DTOs;

public interface IAuthService
{
    AuthResponseDto Login(LoginDto dto);
    AuthResponseDto Register(RegisterDto dto);
    AuthResponseDto VerifyEmail(string token);
    AuthResponseDto ForgotPassword(string email);
    AuthResponseDto ResetPassword(string token, string newPassword);
}