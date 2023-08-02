using RestApi.Interfaces.Authentication;
using RestApi.Models;
using RestApi.Models.Requests;

namespace RestApi.Contexts.Authentication
{
    public class GetUserByEmailContext
    {
        private readonly IGetUserByEmailGateway _dataGateway;

        public GetUserByEmailContext(IGetUserByEmailGateway dataGateway)
        {
            _dataGateway = dataGateway;
        }

        public User Execute(string email)
        {
            try
            {
                return _dataGateway.GetUserByEmail(email);
            }
            catch
            {
                return null;
            }
        }
    }
}