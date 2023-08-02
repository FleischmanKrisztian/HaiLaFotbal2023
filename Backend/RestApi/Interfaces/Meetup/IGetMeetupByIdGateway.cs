namespace RestApi.Interfaces.Meetup
{
    public interface IGetMeetupByIdGateway
    {
        public Models.Meetup GetMeetupById(int id);
    }
}