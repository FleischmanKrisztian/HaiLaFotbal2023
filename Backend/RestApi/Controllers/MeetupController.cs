using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestApi.Contexts.Authentication;
using RestApi.Gateways.Authentication;
using RestApi.Models;
using System.Security.Claims;

namespace RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetupController : ControllerBase
    {
        private readonly GetAllMeetupsContext _getAllMeetupsContext;
        private readonly GetMeetupByIdContext _getMeetupByIdContext;
        private readonly CreateMeetupContext _createMeetupContext;
        private readonly UpdateMeetupContext _updateMeetupContext;
        private readonly DeleteMeetupContext _deleteMeetupContext;

        public MeetupController()
        {
            _getAllMeetupsContext = new GetAllMeetupsContext(new GetAllMeetupsGateway());
            _getMeetupByIdContext = new GetMeetupByIdContext(new GetMeetupByIdGateway());
            _createMeetupContext = new CreateMeetupContext(new CreateMeetupGateway());
            _updateMeetupContext = new UpdateMeetupContext(new UpdateMeetupGateway());
            _deleteMeetupContext = new DeleteMeetupContext(new DeleteMeetupGateway());
        }

        [HttpGet("GetAllMeetups")]
        [Authorize]
        public JsonResult Get()
        {
            return _getAllMeetupsContext.Execute();
        }

        [HttpGet("GetMeetupById")]
        [Authorize]
        public JsonResult GetById(int id)
        {
            return _getMeetupByIdContext.Execute(id);
        }

        [HttpPost("CreateMeetup")]
        [Authorize]
        public JsonResult Post(Meetup meetup)
        {
            var currentUser = GetCurrentUser();
            meetup.CreatedBy.Id = currentUser.Id;
            return _createMeetupContext.Execute(meetup);
        }

        [HttpPut("UpdateMeetup")]
        [Authorize]
        public JsonResult Put(Meetup meetup)
        {
            var currentUser = GetCurrentUser();
            return _updateMeetupContext.Execute(currentUser, meetup);
        }

        [HttpDelete("DeleteMeetup")]
        [Authorize]
        public JsonResult Delete(int id)
        {
            var currentUser = GetCurrentUser();
            return _deleteMeetupContext.Execute(currentUser, id);
        }

        [HttpGet("Admins")]
        [Authorize]
        public IActionResult AdminsEndpoint()
        {
            var currentUser = GetCurrentUser();

            return Ok($"Hi {currentUser.Lastname}, you re Id {currentUser.Id}");
        }

        [HttpGet("Public")]
        public IActionResult Public()
        {
            return Ok("Hi, you're on public property");
        }

        private User GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new User
                {
                    Id = Int32.Parse(userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Sid)?.Value),
                    Email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                    Lastname = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,
                };
            }
            return null;
        }
    }
}