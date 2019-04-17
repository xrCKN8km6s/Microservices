﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Users.DTO;
using Users.Queries;

namespace Users.Controllers
{
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
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile(string sub)
        {
            var res = await _queries.GetUserProfileAsync(sub);

            if (res == null)
            {
                return NotFound($"User profile ${sub} was not found.");
            }

            return Ok(res);
        }
    }
}