using Microsoft.AspNetCore.Mvc;
using migrapp_api.Data;
using migrapp_api.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Claims;
using migrapp_api.Services.User;

[ApiController]
[Route("api/otp")]
public class OtpController : ControllerBase
{
    private readonly IOtpCode _otpService;
    private readonly ApplicationDbContext _appDbContext;
    public OtpController(IOtpCode otpService, ApplicationDbContext appDbContext)
    {
        _otpService = otpService;
        _appDbContext = appDbContext;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateOtp([FromBody] GenerateOtpRequest request)
    {

        var userFound = await _appDbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == request.PasswordHash);

        if (userFound == null)
        {
            return NotFound("Usuario no encontrado");
        }


        if (userFound.PasswordHash != request.PasswordHash)
        {
            return Unauthorized("Credenciales inválidas");
        }


        var otpCode = _otpService.GenerateOtp(userFound.OtpSecretKey);

        return Ok(new { OtpCode = otpCode });
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateOtp([FromBody] ValidateOtpRequest request)
    {
        var userFound = await _appDbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == request.PasswordHash);

        if (userFound == null)
        {
            return NotFound("Usuario no encontrado");
        }


        if (userFound.PasswordHash != request.PasswordHash)
        {
            return Unauthorized("Credenciales inválidas");
        }

        var isValid = _otpService.ValidateOtp(userFound.OtpSecretKey, request.OtpCode);
        return Ok(new { IsValid = isValid });
    }

    public class GenerateOtpRequest
    {
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
    }

    public class ValidateOtpRequest
    {
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string OtpCode { get; set; }
    }
}