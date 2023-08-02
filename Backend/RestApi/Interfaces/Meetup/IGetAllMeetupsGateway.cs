using RestApi.Models;

namespace RestApi.Interfaces.Meetup
{
    public interface IGetAllMeetupsGateway
    {
        public List<Models.Meetup> GetMeetups();
    }
}