namespace RestApi.Models.Responses
{
    public class AuthenticationUserResponse : BaseClass
    {
        public int Id { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        public string RefreshToken { get; set; }

        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }

        public AuthenticationUserResponse()
        { }

        public AuthenticationUserResponse(int id)
        {
            Id = id;
        }

        public AuthenticationUserResponse(bool hasError, string error)
        {
            HasError = hasError;

            Error = error;
        }

        public AuthenticationUserResponse(int id, string lastname, string email)
        {
            Id = id;
            Lastname = lastname;
            Email = email;
        }

        public AuthenticationUserResponse(User user, RefreshToken tokenDetails)
        {
            Id = user.Id;
            Lastname = user.Lastname;
            Email = user.Email;
            RefreshToken = tokenDetails.Token;
            TokenCreated = tokenDetails.Created;
            TokenExpires = tokenDetails.Expires;
        }
    }
}