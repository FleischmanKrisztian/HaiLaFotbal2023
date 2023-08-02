namespace RestApi.Models.Requests
{
    public class UserRegistrationRequest
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public long Birthdate { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmedPassword { get; set; }

        public string PhotoFilename { get; set; }
    }
}