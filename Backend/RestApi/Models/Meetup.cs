namespace RestApi.Models
{
    public class Meetup
    {
        public int Id { get; set; }

        public int Type { get; set; }

        public long MeetingTime { get; set; }

        public string Ycoordinate { get; set; }

        public string Xcoordinate { get; set; }

        public string Description { get; set; }

        public int AvailableSlots { get; set; }

        public int Level { get; set; }

        public long CreatedOn { get; set; }
        
        public User CreatedBy { get; set; }
    }
}