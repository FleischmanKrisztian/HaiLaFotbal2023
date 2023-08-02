using Microsoft.AspNetCore.Mvc;
using RestApi.Interfaces.Authentication;
using RestApi.Interfaces.Meetup;
using RestApi.Models;
using RestApi.Models.Requests;

namespace RestApi.Contexts.Authentication
{
    public class GetAllMeetupsContext
    {
        private readonly IGetAllMeetupsGateway _dataGateway;

        public GetAllMeetupsContext(IGetAllMeetupsGateway dataGateway)
        {
            _dataGateway = dataGateway;
        }

        public JsonResult Execute()
        {
            try
            {
                return new JsonResult(_dataGateway.GetMeetups());
            }
            catch
            {
                var result = new JsonResult("Getting Meetups Failed!");
                result.StatusCode = 400;
                return result;
            }
        }
    }
}