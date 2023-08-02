using RestApi.Interfaces.Authentication;
using RestApi.Models.Requests;
using RestApi.Models.Responses;
using RestApi.Utils;
using System.Text.RegularExpressions;

namespace RestApi.Contexts.AuthenticationContexts
{
    public class RegisterUserContext
    {
        private readonly IRegisterUserGateway _dataGateway;

        public RegisterUserContext(IRegisterUserGateway dataGateway)
        {
            _dataGateway = dataGateway;
        }

        public AuthenticationUserResponse Execute(UserRegistrationRequest userRegistration)
        {
            try
            {
                if (PasswordsDoNotMatch(userRegistration))
                    return new AuthenticationUserResponse(true, "Passwords do not match!");
                if (EmailAddressInvalid(userRegistration))
                    return new AuthenticationUserResponse(true, "Email address is invalid!");
                HashPassword(userRegistration);
                var response = _dataGateway.RegisterUser(userRegistration);
                response.Email = userRegistration.Email;
                response.Lastname = userRegistration.Lastname;

                return response;
            }
            catch
            {
                return new AuthenticationUserResponse(true, "This email address already exists!");
            }
        }

        private void HashPassword(UserRegistrationRequest userRegistration)
        {
            userRegistration.Password = GetHashedString.Execute(userRegistration.Password);
        }

        private static bool EmailAddressInvalid(UserRegistrationRequest userRegistration)
        {
            var pattern = @"^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";

            var regex = new Regex(pattern);
            return !regex.IsMatch(userRegistration.Email);
        }

        private static bool PasswordsDoNotMatch(UserRegistrationRequest userRegistration)
        {
            if (userRegistration.Password != userRegistration.ConfirmedPassword)
                return true;
            return false;
        }
    }
}