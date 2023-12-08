using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DISProject.Api.Models.DTO;
using DISProject.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DISProject.Api.Controllers;

[ApiController]
[Route($"auth")]
public class AuthController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ITokenLifetimeManager _tokenLifetimeManager;

    public AuthController(IConfiguration configuration, ITokenLifetimeManager tokenLifetimeManager)
    {
        _tokenLifetimeManager = tokenLifetimeManager;
        _configuration = configuration;
    }
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginDataRequest loginData)
    {
        if (loginData is { Login: "0000", Password: "0000" })
            return BadRequest("Incorrect login data");
        
        string userName = "Іван";
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userName),
        };
        var jwt = new JwtSecurityToken(
            issuer: "MyIssuer",
            audience: "MyClient",
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromHours(1)),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings:Key").Value)), SecurityAlgorithms.HmacSha256));

        string token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return Ok(new {userName, token});
    }
}