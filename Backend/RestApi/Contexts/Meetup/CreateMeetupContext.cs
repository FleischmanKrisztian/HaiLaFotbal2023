using Microsoft.AspNetCore.Mvc;
using RestApi.Interfaces.Meetup;
using RestApi.Models;

namespace RestApi.Contexts.Authentication
{
    public class CreateMeetupContext
    {
        private readonly ICreateMeetupGateway _dataGateway;

        public CreateMeetupContext(ICreateMeetupGateway dataGateway)
        {
            _dataGateway = dataGateway;
        }

        public JsonResult Execute(Meetup meetup)
        {
            try
            {
                _dataGateway.CreateMeetup(meetup);
                return new JsonResult("Added Successfully");
            }
            catch
            {
                var result = new JsonResult("Add Failed!");
                result.StatusCode = 400;
                return result;
            }
        }
    }
}