namespace RestApi.Interfaces.Meetup
{
    public interface IUpdateMeetupGateway
    {
        public void UpdateMeetup(int id, Models.Meetup meetup);
    }
}