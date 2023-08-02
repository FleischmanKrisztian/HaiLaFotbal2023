using Microsoft.AspNetCore.Mvc;
using RestApi.Interfaces.Meetup;
using RestApi.Models;

namespace RestApi.Contexts.Authentication
{
    public class DeleteMeetupContext
    {
        private readonly IDeleteMeetupGateway _dataGateway;

        public DeleteMeetupContext(IDeleteMeetupGateway dataGateway)
        {
            _dataGateway = dataGateway;
        }

        public JsonResult Execute(User user, int id)
        {
            try
            {
                _dataGateway.DeleteMeetup(user.Id, id);
                return new JsonResult("Deleted Successfully");
            }
            catch
            {
                var result = new JsonResult("Delete Failed!");
                result.StatusCode = 400;
                return result;
            }
        }
    }
}