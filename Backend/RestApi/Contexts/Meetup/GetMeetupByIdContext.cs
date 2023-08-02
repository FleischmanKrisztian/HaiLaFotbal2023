using Microsoft.AspNetCore.Mvc;
using RestApi.Interfaces.Meetup;

namespace RestApi.Contexts.Authentication
{
    public class GetMeetupByIdContext
    {
        private readonly IGetMeetupByIdGateway _dataGateway;

        public GetMeetupByIdContext(IGetMeetupByIdGateway dataGateway)
        {
            _dataGateway = dataGateway;
        }

        public JsonResult Execute(int id)
        {
            try
            {
                return new JsonResult(_dataGateway.GetMeetupById(id));
            }
            catch
            {
                var result = new JsonResult("Getting Meetup Failed!");
                result.StatusCode = 400;
                return result;
            }
        }
    }
}