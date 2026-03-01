namespace AuthService.Application.DTOs;

public class AuthResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }

    public static AuthResponseDto SuccessResponse(string message, string? token = null)
    {
        return new AuthResponseDto
        {
            Success = true,
            Message = message,
            Token = token
        };
    }

    public static AuthResponseDto Fail(string message)
    {
        return new AuthResponseDto
        {
            Success = false,
            Message = message
        };
    }
}