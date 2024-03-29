using Microsoft.AspNetCore.Mvc;
using Users.API.DTO;
using Users.API.Queries;

namespace Users.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly IUsersQueries _queries;

    public ProfileController(IUsersQueries queries)
    {
        _queries = queries ?? throw new ArgumentNullException(nameof(queries));
    }

    [HttpGet("{sub}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<UserProfileDto>> GetUserProfile(string sub)
    {
        var res = await _queries.GetUserProfileAsync(sub);

        if (res == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"User profile {sub} was not found.");
        }

        return Ok(res);
    }
}
