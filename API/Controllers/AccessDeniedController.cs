using DAL.DbAccess;
using Microsoft.AspNetCore.Mvc;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

/// <summary>
/// The world's single most pointless controller.
/// Just returned 401 unauthorized when the user is forbidden due to the auth middleware, so it won't be a 404.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AccessDeniedController: Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        return Unauthorized(FormatErrorMessage(NotLoggedIn,
            CustomStatusCode.NotLoggedIn));
    }
}