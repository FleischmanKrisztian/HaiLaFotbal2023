namespace RestApi.Interfaces.Meetup
{
    public interface ICreateMeetupGateway
    {
        public void CreateMeetup(Models.Meetup meetup);
    }
}