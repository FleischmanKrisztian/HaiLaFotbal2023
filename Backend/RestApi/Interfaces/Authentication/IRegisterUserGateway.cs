using RestApi.Models.Requests;
using RestApi.Models.Responses;

namespace RestApi.Interfaces.Authentication
{
    public interface IRegisterUserGateway
    {
        public AuthenticationUserResponse RegisterUser(UserRegistrationRequest user);
    }
}