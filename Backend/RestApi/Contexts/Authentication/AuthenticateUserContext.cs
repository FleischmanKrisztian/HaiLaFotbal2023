using RestApi.Interfaces.Authentication;
using RestApi.Models;
using RestApi.Models.Requests;
using RestApi.Models.Responses;
using RestApi.Utils;

namespace RestApi.Contexts.AuthenticationContexts
{
    public class AuthenticateUserContext
    {
        private readonly IGetUserByEmailGateway _dataGateway;

        public AuthenticateUserContext(IGetUserByEmailGateway dataGateway)
        {
            _dataGateway = dataGateway;
        }

        public AuthenticationUserResponse Execute(UserLoginRequest userLogin)
        {
            try
            {
                var user = _dataGateway.GetUserByEmail(userLogin.Email);
                if (user == null)
                {
                    return new AuthenticationUserResponse(true, "The Email address you entered was incorrect!"); ;
                }
                if (IsLoginInfoValid(user, userLogin))
                {
                    return new AuthenticationUserResponse
                    {
                        Id = user.Id,
                        Lastname = user.Lastname,
                        Email = user.Email,                          
                    };
                }
                return new AuthenticationUserResponse(true, "The password you entered was incorrect!");
            }
            catch
            {
                return new AuthenticationUserResponse(true, "An Error has Occured");
            }
        }

        private static bool IsLoginInfoValid(User user, UserLoginRequest userLogin)
        {
            if (user.HashedPassword == GetHashedString.Execute(userLogin.Password))
            {
                return true;
            }
            return false;
        }
    }
}