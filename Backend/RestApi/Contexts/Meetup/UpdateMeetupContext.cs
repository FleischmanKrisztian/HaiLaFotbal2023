using Microsoft.AspNetCore.Mvc;
using RestApi.Interfaces.Meetup;
using RestApi.Models;

namespace RestApi.Contexts.Authentication
{
    public class UpdateMeetupContext
    {
        private readonly IUpdateMeetupGateway _dataGateway;

        public UpdateMeetupContext(IUpdateMeetupGateway dataGateway)
        {
            _dataGateway = dataGateway;
        }

        public JsonResult Execute(User user, Meetup meetup)
        {
            try
            {
                _dataGateway.UpdateMeetup(user.Id, meetup);
                return new JsonResult("Updated Successfully!");
            }
            catch
            {
                var result = new JsonResult("Update Failed!");
                result.StatusCode = 400;
                return result;
            }
        }
    }
}