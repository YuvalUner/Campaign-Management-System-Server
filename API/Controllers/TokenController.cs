using System.Security.Claims;
using System.Text;
using API.SessionExtensions;
using API.Utils;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// A controller for logging in and out.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TokenController : Controller
{
    private readonly IConfigurationSection _googleSettings;
    private readonly IUsersService _usersService;
    private readonly ILogger<TokenController> _logger;
    
    public TokenController(IConfiguration configuration, IUsersService usersService, ILogger<TokenController> logger)
    {
        _googleSettings = configuration.GetSection("Authentication:Google");
        _usersService = usersService;
        _logger = logger;
    }
    
    
    /// <summary>
    /// Verifies the validity of the token and returns the user's data
    /// </summary>
    /// <param name="IdToken">Google JWT token sent by the client</param>
    /// <returns>Token's payload on success, null on failure</returns>
    private async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(ExternalAuthDto externalAuth)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _googleSettings.GetSection("ClientId").Value }
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
            return payload;
        }
        catch (Exception ex)
        {
            //log an exception
            _logger.LogInformation("Client with IP {IpAddr} tried to log in with an invalid token", 
                HttpContext.Connection.RemoteIpAddress);
            return null;
        }
    }
    
    /// <summary>
    /// API function for logging a user in via their google account.<br/>
    /// Creates a new user if they don't exist in the database.<br/>
    /// Sets cookies and session data for the user.
    /// </summary>
    /// <param name="externalAuth">A <see cref="ExternalAuthDto"/> containing the token and provider.</param>
    /// <returns>BadRequest if the token is not valid, 500 if something failed in user creation (should not happen),
    /// Ok otherwise.</returns>
    [HttpPost("googleSignIn")]
    public async Task<IActionResult> SignUserIn([FromBody] ExternalAuthDto externalAuth)
    {
        try
        {
            // Verify Google token
            var payload = await VerifyGoogleToken(externalAuth);
            if (payload == null)
            {
                return BadRequest();
            }

            // Get user from DB. If user doesn't exist, create a new one
            var user = await _usersService.GetUserByEmail(payload.Email);
            
            bool created = false;

            if (user == null)
            {
                user = new User()
                {
                    Email = payload.Email,
                    DisplayNameEng = payload.Name,
                    FirstNameEng = payload.GivenName,
                    LastNameEng = payload.FamilyName,
                    ProfilePicUrl = payload.Picture,
                    UserId = 0
                };
                int userId = await _usersService.CreateUser(user);
                // Theoretically, this should never happen
                // In case it does, log it as critical and return 500, because something is wrong with the DB
                if (userId == -1)
                {
                    _logger.LogCritical("User creation failed for email {Email}", payload.Email);
                    return StatusCode(500);
                }

                user.UserId = userId;
                created = true;
                _logger.LogInformation("Created new user with email {Email}", payload.Email);
            }

            // Get some frequently used info about the user that will be stored in the session for use in future requests
            // without needing to query the database each time for them.
            user.Authenticated = await _usersService.IsUserAuthenticated(user.UserId);
            List<CampaignUser> userCampaigns = await _usersService.GetUserCampaigns(user.UserId);
            var allowedCampaignGuids = userCampaigns.Select(c => c.CampaignGuid).ToList();

            // Log the user in via cookie authentication and session
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FirstNameEng),
                new Claim("FullName", user.DisplayNameEng),
                new Claim(ClaimTypes.Surname, user.LastNameEng),
            };

            // Copy pasted code from
            // https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-7.0
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
                IssuedUtc = DateTimeOffset.Now,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            // Keep the user id in the session for future use, as it should not be exposed to the client via claims
            HttpContext.Session.SetInt32(Constants.UserId, user.UserId);
            // Store whether the user is authenticated or not, to avoid unnecessary DB calls
            HttpContext.Session.Set(Constants.UserAuthenticationStatus, user.Authenticated);
            // Store the user's allowed campaigns (campaigns they are a part of), to avoid many unnecessary DB calls
            HttpContext.Session.Set(Constants.AllowedCampaigns, allowedCampaignGuids);

            if (created)
            {
                return Created("", new {});
            }
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while signing in user");
            return StatusCode(500);
        }
    }
    
    /// <summary>
    /// Signs the user out by clearing the session and cookies.
    /// </summary>
    /// <returns>Ok.</returns>
    [HttpPost("signOut")]
    public async Task<IActionResult> SignUserOut()
    {
        try
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while signing out user");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Signs in to my account (Yuval).
    /// Purely for ease of testing, remove this when development is done.
    /// </summary>
    /// <returns></returns>
    [HttpPost("TestSignInRemoveLater")]
    public async Task<IActionResult> TestSignInRemoveLater()
    {
        // The test user - me
        var user = new User()
        {
            UserId = 1,
            Email = "yuvaluner@gmail.com",
            DisplayNameEng = "Yuval Uner",
            FirstNameEng = "Yuval",
            LastNameEng = "Uner"
        };
        
        user.Authenticated = await _usersService.IsUserAuthenticated(user.UserId);
        List<CampaignUser> userCampaigns = await _usersService.GetUserCampaigns(user.UserId);
        var allowedCampaignGuids = userCampaigns.Select(c => c.CampaignGuid).ToList();
        
        // Log the user in via cookie authentication and session
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FirstNameEng),
            new Claim("FullName", user.DisplayNameEng),
            new Claim(ClaimTypes.Surname, user.LastNameEng),
        };

        // Copy pasted code from
        // https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-7.0
        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            IsPersistent = true,
            IssuedUtc = DateTimeOffset.Now,
        };
        
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity), 
            authProperties);
        // Keep the user id in the session for future use, as it should not be exposed to the client via claims
        HttpContext.Session.SetInt32(Constants.UserId, user.UserId);
        // Store whether the user is authenticated or not, to avoid unnecessary DB calls
        HttpContext.Session.Set(Constants.UserAuthenticationStatus, user.Authenticated);
        // Store the user's allowed campaigns (campaigns they are a part of), to avoid many unnecessary DB calls
        HttpContext.Session.Set(Constants.AllowedCampaigns, allowedCampaignGuids);
        return Ok();
    }
}