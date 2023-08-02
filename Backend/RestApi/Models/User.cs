namespace RestApi.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public long Birthdate { get; set; }

        public string Email { get; set; }

        public string HashedPassword { get; set; }

        public long CreatedOn { get; set; }

        public string PhotoFileName { get; set; }

        public User()
        {
        }
    }
}