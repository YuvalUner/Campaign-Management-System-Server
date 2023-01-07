using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JwtController : Controller
{
    private readonly IConfigurationSection _googleSettings;
    private readonly IConfigurationSection _jwtSettings;
    
    public JwtController(IConfiguration configuration)
    {
        _googleSettings = configuration.GetSection("Authentication:Google");
        _jwtSettings = configuration.GetSection("Authentication:JWT");
    }
    
    
    private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string IdToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _googleSettings.GetSection("ClientId").Value }
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(IdToken, settings);
            return payload;
        }
        catch (Exception ex)
        {
            //log an exception
            return null;
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] string idToken)
    {
        var payload = await VerifyGoogleToken(idToken);
        if (payload == null)
        {
            return BadRequest();
        }
    
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, payload.Subject),
            new Claim(ClaimTypes.Name, payload.Name),
            new Claim(ClaimTypes.Email, payload.Email),
            new Claim(ClaimTypes.GivenName, payload.GivenName),
            new Claim(ClaimTypes.Surname, payload.FamilyName),
            new Claim(ClaimTypes.Uri, payload.Picture),
        };
    
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.GetSection("PrivateKey").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.GetSection("Issuer").Value,
            audience: _jwtSettings.GetSection("Audience").Value,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);
    
        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }
}