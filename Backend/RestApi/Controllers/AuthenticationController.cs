using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestApi.Contexts.Authentication;
using RestApi.Contexts.AuthenticationContexts;
using RestApi.Gateways.Authentication;
using RestApi.Models;
using RestApi.Models.Requests;
using RestApi.Models.Responses;
using RestApi.Utils;
using System.Security.Claims;
using System.Security.Cryptography;

namespace RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AuthenticateUserContext _authenticateUserContext;
        private readonly RegisterUserContext _registerUserContext;
        private readonly GetUserByEmailContext _getUserByEmailContext;
        private readonly GetRefreshTokenContext _getRefreshTokenContext;
        private readonly AddOrUpdateRefreshTokenContext _addOrUpdateRefreshTokenContext;

        public AuthenticationController(IConfiguration config)
        {
            _config = config;
            _authenticateUserContext = new AuthenticateUserContext(new GetUserByEmailGateway());
            _registerUserContext = new RegisterUserContext(new RegisterUserGateway());
            _getUserByEmailContext = new GetUserByEmailContext(new GetUserByEmailGateway());
            _getRefreshTokenContext = new GetRefreshTokenContext(new GetRefreshTokenGateway());
            _addOrUpdateRefreshTokenContext = new AddOrUpdateRefreshTokenContext(new AddOrUpdateRefreshTokenGateway());
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserLoginRequest userLogin)
        {
            var userDto = _authenticateUserContext.Execute(userLogin);

            if (userDto.HasError)
            {
                return NotFound(userDto.Error);
            }

            var token = GenerateJwtSecurityToken.Execute(_config, userDto);
            var newRefreshToken = GenerateRefreshToken(userDto.Id);
            SetRefreshToken(userDto, newRefreshToken);
            _addOrUpdateRefreshTokenContext.Execute(newRefreshToken);
            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register([FromBody] UserRegistrationRequest userRegistration)
        {
            var userDto = _registerUserContext.Execute(userRegistration);

            if (userDto.HasError)
            {
                return BadRequest(userDto.Error);
            }

            var token = GenerateJwtSecurityToken.Execute(_config, userDto);
            var newRefreshToken = GenerateRefreshToken(userDto.Id);
            SetRefreshToken(userDto, newRefreshToken);
            _addOrUpdateRefreshTokenContext.Execute(newRefreshToken);
            return Ok(token);
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return BadRequest("Please login before trying to refresh token!");
            }
            var user = _getUserByEmailContext.Execute(currentUser.Email);

            if (user == null)
            {
                return Unauthorized("This Email Address no longer exists!");
            }

            var tokenDetails = _getRefreshTokenContext.Execute(currentUser.Id);

            if (tokenDetails == null)
            {
                return Unauthorized("Token does not Exist!");
            }

            var userDto = new AuthenticationUserResponse(user, tokenDetails);

            var refreshToken = Request.Cookies["refreshToken"];

            if (!userDto.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (userDto.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            var token = GenerateJwtSecurityToken.Execute(_config, userDto);
            var newRefreshToken = GenerateRefreshToken(userDto.Id);
            SetRefreshToken(userDto, newRefreshToken);
            if (!_addOrUpdateRefreshTokenContext.Execute(newRefreshToken))
                return BadRequest("Could Not Add/Update RefreshToken");
            return Ok(token);
        }

        private AuthenticationUserResponse GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity.Claims.Count() != 0)
            {
                var userClaims = identity.Claims;

                return new AuthenticationUserResponse
                {
                    Id = Int32.Parse(userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Sid)?.Value),
                    Email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                    Lastname = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,
                };
            }
            return null;
        }

        private RefreshToken GenerateRefreshToken(int id)
        {
            var refreshToken = new RefreshToken
            {
                Id = id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(AuthenticationUserResponse userDto, RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            userDto.RefreshToken = newRefreshToken.Token;
            userDto.TokenCreated = newRefreshToken.Created;
            userDto.TokenExpires = newRefreshToken.Expires;
        }
    }
}