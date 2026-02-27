using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    // ========================= LOGIN =========================
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var result = _auth.Login(dto);
        return result.Success ? Ok(result) : Unauthorized(result);
    }

    // ========================= REGISTER =========================
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDto dto)
    {
        var result = _auth.Register(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ========================= VERIFY EMAIL =========================
    [HttpPost("verify-email")]
    public IActionResult VerifyEmail([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest(new { success = false, message = "El token es requerido" });

        var result = _auth.VerifyEmail(token);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ========================= FORGOT PASSWORD =========================
    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new { success = false, message = "El email es requerido" });

        var result = _auth.ForgotPassword(email);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ========================= RESET PASSWORD =========================
    [HttpPost("reset-password")]
    public IActionResult ResetPassword(
        [FromQuery] string token,
        [FromQuery] string newPassword)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
            return BadRequest(new { success = false, message = "Token y nueva contraseña son requeridos" });

        var result = _auth.ResetPassword(token, newPassword);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}